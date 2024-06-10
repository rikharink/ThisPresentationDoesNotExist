export async function fetchSlideNotesById(id) {
  return await fetch(`/api/slide/${id}/notes`).then((response) =>
    response.json()
  );
}

export async function fetchSlidePromptsById(id) {
  return await fetch(`/api/slide/${id}/prompts`).then((response) =>
    response.json()
  );
}

export async function fetchSlideImageByPrompt(prompt) {
  const encodedPositivePrompt = encodeURIComponent(prompt.positive);
  const encodedNegativePrompt = encodeURIComponent(prompt.negative ?? "deformed iris, deformed pupils, text, cropped, out of frame, worst quality, low quality, jpeg artifacts, ugly, duplicate, morbid, mutilated, extra fingers, mutated hands, poorly drawn hands, poorly drawn face, mutation, deformed, blurry, dehydrated, bad anatomy, bad proportions, extra limbs, cloned face, disfigured, gross proportions, malformed limbs, missing arms, missing legs, extra arms, extra legs, fused fingers, too many fingers, long neck");
  return await fetch(`/api/slide/image/${encodedPositivePrompt}/${encodedNegativePrompt}`).then((response) =>
    response.blob()
  );
}
