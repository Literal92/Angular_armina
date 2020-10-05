export function DisableLoading() {
   //console.log('disbale')
  //$("#loadingBox").fadeOut(330);
  var preloaderactivate = document.querySelector('.preloader-activate');
  preloaderactivate.classList.remove('preloader-active');
}

export function EnableLoading() {
  //console.log('enable');
  // $("#loadingBox").fadeIn(330);
  var preloaderactivate = document.querySelector('.preloader-activate');
  preloaderactivate.classList.add('preloader-active');


}
