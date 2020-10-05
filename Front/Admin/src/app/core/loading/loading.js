export function DisableLoading() {
   console.log('disbale')
    $("#loadingBox").fadeOut(330)
}

export function EnableLoading() {
   console.log('enable')
   $("#loadingBox").fadeIn(330)
}