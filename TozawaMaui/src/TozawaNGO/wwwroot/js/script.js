function SetScroll(value) {
    var hasScroll = document.body.scrollHeight > document.body.clientHeight;
    if (hasScroll) {
        window.scrollTo(0, value);
    }
}

function checkOverflow(className, dotNetObject) {
    if (className == undefined || className == "") {
        return false;
    }
    var mudTextField = document.getElementsByClassName(className)[0];
    var firstChild = mudTextField.children[0];
    var firstChildOfFirstChild = firstChild.children[0];
    var inputElement = firstChildOfFirstChild.children[0];
    var result = false;
    if (inputElement.offsetWidth < inputElement.scrollWidth) {
        result = true;
    }

    new ResizeObserver(changes => {
        addDescIcon(className, dotNetObject);
    }).observe(inputElement)

    return result;
}
function addDescIcon(className, dotNetObject) {
    var mudTextField = document.getElementsByClassName(className)[0];
    var firstChild = mudTextField.children[0];
    var firstChildOfFirstChild = firstChild.children[0];
    if (firstChildOfFirstChild != undefined && firstChildOfFirstChild.children != undefined) {
        var inputElement = firstChildOfFirstChild.children[0];
        if (inputElement.offsetWidth < inputElement.scrollWidth) {
            dotNetObject.invokeMethodAsync('AddDescIcon', className, false);
        } else {
            dotNetObject.invokeMethodAsync('AddDescIcon', className, true);
        }
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
window.DisabledCopyPasteToPasswordField = (message) => {
    $('.tz_password').bind("cut copy paste", function (e) {
        e.preventDefault();
        alert(message);
        $('.tz_password').bind("contextmenu", function (e) {
            e.preventDefault();
        });
    });
};
function getScreeenSize() {
    var width = window.innerWidth;
    return width;
}
function getWindowSize() {
    var element = document.getElementById("tzMainContainer");
    var width = window.innerWidth;
    if (width <= 980) {
        if (element.classList.contains("pt-16")) {
            element.classList.remove("pt-16")
        }
        if (element.classList.contains("px-16")) {
            element.classList.remove("px-16")
        }
    } else {
        if (!element.classList.contains("pt-16")) {
            element.classList.add("pt-16")
        }
        if (!element.classList.contains("px-16")) {
            element.classList.add("px-16")
        }
    }
}
window.addEventListener("resize", getWindowSize);
window.onload = function () {
    setTimeout(function () {
        getWindowSize();
    }, 700);
};

