export function resizeImgBase64(src, newX, newY) {
  return new Promise((res, rej) => {
    const img = new Image();
    img.src = src;
    img.onload = () => {
      const elem = document.createElement('canvas');
      newX = elem.width > newX ? newX : elem.width;
      newY = elem.height > newY ? newY : elem.height;

      elem.width = newX;
      elem.height = newY;
      const ctx = elem.getContext('2d');
      ctx.drawImage(img, 0, 0, newX, newY);
      const data = ctx.canvas.toDataURL();
      res(data);
    };
    img.onerror = error => rej(error);
  });
}


