export function modal(id,value) {
  if (value === false) {
    $(id).modal('hide');
  }
  else {
    $(id).modal();
  }
}
