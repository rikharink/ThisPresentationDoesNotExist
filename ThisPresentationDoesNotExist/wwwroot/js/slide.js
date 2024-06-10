import {
  fetchSlidePromptsById,
  fetchSlideImageByPrompt,
} from "./slide-repository.js";

import { connection } from "./connection.js";
import { debounce } from "./util.js";
import { marked } from "https://cdn.jsdelivr.net/npm/marked/lib/marked.esm.js";

export class Slide {
  constructor(id) {
    this.id = id;
    this.promptEditor = document.getElementById("prompt-editor");
    this.textPromptInput = document.getElementById("text-prompt");
    this.imagePromptInput = document.getElementById("image-prompt");
    this.slide = document.getElementById("slide");
    this.slideContent = document.getElementById("slide-content");
    this.slideImage = document.getElementById("slide-image");

    document.addEventListener("keyup", async (e) => {
      if (e.key === "F9") {
        this.promptEditor.classList.toggle("hidden");
        this.showThoughts(!this.promptEditor.classList.contains("hidden"));
      }
      if (e.key === "F8") {
        await fetch("/api/slide/text/reset");
        console.info("Context reset");
      }
    });
  }
  
  showThoughts(show) {
    let thoughts = document.getElementsByTagName("thinking");
    if(show) {
      for (let thought of thoughts) {
        thought.style.display = "block";
      }
    } else {
      for (let thought of thoughts) {
        thought.style.display = "none";
      }
    }
  }
  
  async update(id, shouldSync = true) {
    this.id = id;
    history.pushState({}, null, id);
    
    if (shouldSync) {
      await connection.invoke("SyncSlide", id);
    }
    
    await this.init();
  }

  async init() {
    this.prompts = await fetchSlidePromptsById(this.id);
    

    if (this.prompts.imagePrompt) {
      this.imagePromptInput.innerText = this.prompts.imagePrompt.positive;
    } else {
      this.imagePromptInput.innerText = "";
      this.slideImage.innerHTML = "";
    }
    
    if (this.prompts.textPrompt) {
      this.textPromptInput.innerText = this.prompts.textPrompt;
    } else {
      this.textPromptInput.innerText = "";
      this.slideContent.innerHTML = "";
    }
    
    this.refreshSplit();
    
    if(this.imagePromptInput.innerText) {
      await this.refreshImageContent(this.prompts.imagePrompt);
    }
    if(this.textPromptInput.innerText){
      await this.refreshTextContent(this.prompts.textPrompt);
    }

    this.textPromptInput.addEventListener(
        "input",
        debounce(
            async (e) => await this.refreshTextContent(e.target.innerText),
            1000
        )
    );
    this.imagePromptInput.addEventListener(
        "input",
        debounce(
            async (e) => await this.refreshImageContent({ positive: e.target.innerText }),
            1000
        )
    );
  }

  async refreshTextContent(prompt) {
    const encodedPrompt = encodeURIComponent(prompt);
    this.text = "";
    await fetch(`/api/slide/text/${encodedPrompt}`).then(async (response) => {
      this.slideContent.classList.remove("hidden");
      this.refreshSplit();
      const reader = response.body
        .pipeThrough(new TextDecoderStream())
        .getReader();
      while (true) {
        const { done, value } = await reader.read();
        if (done) {
          break;
        }
        this.text += value;
        this.slideContent.innerHTML = marked.parse(this.text);
      }
    });
    this.refreshSplit();
  }

  async refreshImageContent(prompt) {
    const blob = await fetchSlideImageByPrompt(prompt);
    const src = URL.createObjectURL(blob);
    const img = document.createElement("img");
    img.src = src;
    this.slideImage.innerHTML = "";
    this.slideImage.appendChild(img);
    this.slideImage.classList.remove("hidden");
    this.refreshSplit();
  }

  refreshSplit() {
    if (this.imagePromptInput.innerText && this.textPromptInput.innerText) {
      this.slide.classList.add("split");
    } else {
      this.slide.classList.remove("split");
    }
    
    if(!this.imagePromptInput.innerText) {
      this.slideImage.classList.add("hidden");
    }
    if(!this.textPromptInput.innerText) {
      this.slideContent.classList.add("hidden");
    }
  }
}
