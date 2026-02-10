mergeInto(LibraryManager.library, {
  IsMobileDevice: function () {
    return /Android|iPhone|iPad|iPod|Windows Phone/i.test(navigator.userAgent);
  }
});
