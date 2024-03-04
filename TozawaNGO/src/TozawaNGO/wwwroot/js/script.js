function SetScroll(value) {
    var hasScroll = document.body.scrollHeight > document.body.clientHeight;
    if (hasScroll && document.documentElement && document.documentElement.scrollTop) {
        console.log(value);
        console.log(document.documentElement.scrollTop);
        document.documentElement.scrollTop = value;
    }
}

function RemoveScroll() {
    var hasScroll = document.body.scrollHeight > document.body.clientHeight;
    if (hasScroll && document.documentElement && document.documentElement.scrollTop) {
        console.log(value);
        console.log(document.documentElement.scrollTop);
        document.documentElement.scrollTop = 0;
    }
}

function saveAsFile(filename, bytesBase64) {
    if (navigator.msSaveBlob) {
        //Download document in Edge browser
        var data = window.atob(bytesBase64);
        var bytes = new Uint8Array(data.length);
        for (var i = 0; i < data.length; i++) {
            bytes[i] = data.charCodeAt(i);
        }
        var blob = new Blob([bytes.buffer], { type: "application/octet-stream" });
        navigator.msSaveBlob(blob, filename);
    }
    else {
        var link = document.createElement('a');
        link.download = filename;
        link.href = "data:application/octet-stream;base64," + bytesBase64;
        document.body.appendChild(link); // Needed for Firefox
        link.click();
        document.body.removeChild(link);
    }
}