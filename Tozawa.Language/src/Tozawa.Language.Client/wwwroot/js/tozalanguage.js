function download(options) {
    var url = b64ToBlobUrl(options.byteArray, "text/plain", 512);
    var anchorElement = document.createElement('a');
    anchorElement.href = url;
    anchorElement.download = options.fileName;
    anchorElement.click();
    anchorElement.remove();
}

function b64ToBlobUrl(b64Data, contentType, sliceSize) {
    contentType = contentType || "";
    sliceSize = sliceSize || 512;
    var byteCharacters = atob(b64Data);
    var byteArrays = [];
    for (var offset = 0; offset < byteCharacters.length; offset += sliceSize) {
        var slice = byteCharacters.slice(offset, offset + sliceSize);
        var byteNumbers = new Array(slice.length);
        for (var i = 0; i < slice.length; i++) {
            byteNumbers[i] = slice.charCodeAt(i);
        }
        var byteArray = new Uint8Array(byteNumbers); byteArrays.push(byteArray);
    }
    var blob = new Blob(byteArrays, { type: contentType });
    return URL.createObjectURL(blob);
}
