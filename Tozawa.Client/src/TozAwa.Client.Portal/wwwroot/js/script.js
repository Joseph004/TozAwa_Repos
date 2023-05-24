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

window.AddTitleToInnerIconLoginAsAdmin = (message) => {
    $('.tz_loginAsAdmin .mud-input-adornment-end .mud-icon-button-edge-end').attr('title', message);
};
window.AddTitleToInnerIconLoginAsNotAdmin = (message) => {
    $('.tz_loginAsNotAdmin .mud-input-adornment-end .mud-icon-button-edge-end').attr('title', message);
};
window.AddTextTest = () => {
    $('#tz-add').on("click", function () {
        $("<p>Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Vestibulum tortor quam, feugiat vitae, ultricies eget, tempor sit amet, ante. Donec eu libero sit amet quam egestas semper. Aenean ultricies mi vitae est. Mauris placerat eleifend leo.</p>").appendTo(".tz-content");
    });
};
window.AddTitleToInnerEmailIconSend = (message) => {
    $('.tz-email-foot .mud-input-adornment-end .mud-icon-button-edge-end').attr('title', message);
};
function FooterSetToColumn(dotnethelper) {
    resizeFoot();
    window.addEventListener('resize', resizeFoot);
}
function sameMethodResize() {
    var values = document.getElementsByClassName("flexColumn-main");
    Array.from(values).forEach(function (item) {

        var className = item.className;
        var ua = window.navigator.userAgent;
        var msie = ua.indexOf("MSIE ");

        if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./))  // If Internet Explorer, return version number
        {
            if (className.split(' ').includes("flex-column")) {
                var newClass = "";
                Array.from(className.split(' ')).forEach(function (itemClass) {
                    if (itemClass != "flex-column") {
                        if (newClass == "") {
                            newClass = itemClass;
                        } else {
                            newClass += ` ${itemClass}`;
                        }
                    }
                });
                item.className = "";
                item.className = newClass;
            }

            var valueSpaces = document.getElementsByClassName("flexColumn");
            Array.from(valueSpaces).forEach(function (itemSpace) {
                var clsName = itemSpace.className;

                if (clsName.split(' ').includes("tz-div-space")) {
                    var newClassSp = "";
                    Array.from(clsName.split(' ')).forEach(function (itemClassSp) {
                        if (itemClassSp != "tz-div-space") {
                            if (newClassSp == "") {
                                newClassSp = itemClassSp;
                            } else {
                                newClassSp += ` ${itemClassSp}`;
                            }
                        }
                    });
                    itemSpace.className = "";
                    itemSpace.className = newClassSp;
                }
            });
        }
        else  // If another browser, return 0
        {
            if (className.split(' ').includes("flex-column")) {
                item.classList.remove("flex-column");
            }

            var valueSpaces = document.getElementsByClassName("flexColumn");
            Array.from(valueSpaces).forEach(function (itemSpace) {
                var clsName = itemSpace.className;
                if (!clsName.split(' ').includes("tz-div-space")) {
                    itemSpace.classList.add("tz-div-space");
                }
            });
        }
    });
    $('.tz-div-space:not(:first-child):not(:last-child)').css("margin-left", "8vh");
    $('.tz-div-space:not(:first-child):not(:last-child)').css("margin-right", "8vh");
    $('.tz-div-space:not(:first-child):not(:last-child)').css("margin-top", "");
    $('.tz-div-space:not(:first-child):not(:last-child)').css("margin-bottom", "");

    $('.tz-div-space-icontext:not(:first-child):not(:last-child)').css("margin-left", "8vh");
    $('.tz-div-space-icontext:not(:first-child):not(:last-child)').css("margin-right", "8vh");
    $('.tz-div-space-icontext:not(:first-child):not(:last-child)').css("margin-top", "");
    $('.tz-div-space-icontext:not(:first-child):not(:last-child)').css("margin-bottom", "");
}
function resizeFoot() {
    var width = window.innerWidth
        || document.documentElement.clientWidth
        || document.body.clientWidth;

    var height = window.innerHeight
        || document.documentElement.clientHeight
        || document.body.clientHeight;

    if (width <= 1448 && width >= 1100) {
        sameMethodResize();

        $('.tz-max-div-text').css("max-width", "25vh");
        $('.tz-max-div-text').css("width", "25vh");

        $('.tz-email-foot').css("width", "25vh");
    }
    if (width <= 1100 && width >= 948) {
        sameMethodResize();

        $('.tz-max-div-text').css("max-width", "15vh");
        $('.tz-max-div-text').css("width", "15vh");

        $('.tz-email-foot').css("width", "15vh");
    }
    if (width <= 948 && width >= 715) {
        sameMethodResize();

        $('.tz-max-div-text').css("max-width", "10vh");
        $('.tz-max-div-text').css("width", "10vh");

        $('.tz-email-foot').css("width", "18vh");
    }
    if (width <= 715) {
        var values = document.getElementsByClassName("flexColumn-main");
        Array.from(values).forEach(function (item) {

            var className = item.className;
            var ua = window.navigator.userAgent;
            var msie = ua.indexOf("MSIE ");

            if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./))  // If Internet Explorer, return version number
            {
                if (!className.split(' ').includes("flex-column")) {
                    className += " flex-column";
                }

                var valueSpaces = document.getElementsByClassName("flexColumn");
                Array.from(valueSpaces).forEach(function (itemSpace) {
                    var clsName = itemSpace.className;

                    if (clsName.split(' ').includes("tz-div-space")) {
                        var newClass = "";
                        Array.from(clsName.split(' ')).forEach(function (itemClass) {
                            if (itemClass != "tz-div-space") {
                                if (newClass == "") {
                                    newClass = itemClass;
                                } else {
                                    newClass += ` ${itemClass}`;
                                }
                            }
                        });
                        itemSpace.className = "";
                        itemSpace.className = newClass;
                    }
                });
            }
            else  // If another browser, return 0
            {
                if (!className.split(' ').includes("flex-column")) {
                    item.classList.add("flex-column");
                }

                /* var valueSpaces = document.getElementsByClassName("flexColumn");
                Array.from(valueSpaces).forEach(function (itemSpace) {
                    var clsName = itemSpace.className;
                    if (clsName.split(' ').includes("tz-div-space")) {
                        itemSpace.classList.remove("tz-div-space");
                    }
                }); */
            }
        });
        $('.tz-div-space:not(:first-child):not(:last-child)').css("margin-left", "0vh");
        $('.tz-div-space:not(:first-child):not(:last-child)').css("margin-right", "0vh");
        $('.tz-div-space:not(:first-child):not(:last-child)').css("margin-top", "0vh");
        $('.tz-div-space:not(:first-child):not(:last-child)').css("margin-bottom", "0vh");

        $('.tz-div-space-icontext:not(:first-child):not(:last-child)').css("margin-left", "0vh");
        $('.tz-div-space-icontext:not(:first-child):not(:last-child)').css("margin-right", "0vh");
        $('.tz-div-space-icontext:not(:first-child):not(:last-child)').css("margin-top", "0vh");
        $('.tz-div-space-icontext:not(:first-child):not(:last-child)').css("margin-bottom", "0vh");

        $('.tz-max-div-text').css("max-width", "30vh");
        $('.tz-max-div-text').css("width", "30vh");

        $('.tz-email-foot').css("width", "30vh");
    }
    if (width >= 1448) {
        var values = document.getElementsByClassName("flexColumn-main");
        for (let i = 0; i < values.length; i++) {
            var item = values[i];

            var className = item.className;
            var ua = window.navigator.userAgent;
            var msie = ua.indexOf("MSIE ");

            if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./))  // If Internet Explorer, return version number
            {
                if (className.split(' ').includes("flex-column")) {
                    var newClass = "";
                    Array.from(className.split(' ')).forEach(function (itemClass) {
                        if (itemClass != "flex-column") {
                            if (newClass == "") {
                                newClass = itemClass;
                            } else {
                                newClass += ` ${itemClass}`;
                            }
                        }
                    });
                    item.className = "";
                    item.className = newClass;
                }

                var valueSpaces = document.getElementsByClassName("flexColumn");
                Array.from(valueSpaces).forEach(function (itemSpace) {
                    var clsName = itemSpace.className;

                    if (clsName.split(' ').includes("tz-div-space")) {
                        var newClassSp = "";
                        Array.from(clsName.split(' ')).forEach(function (itemClassSp) {
                            if (itemClassSp != "tz-div-space") {
                                if (newClassSp == "") {
                                    newClassSp = itemClassSp;
                                } else {
                                    newClassSp += ` ${itemClassSp}`;
                                }
                            }
                        });
                        itemSpace.className = "";
                        itemSpace.className = newClassSp;
                    }
                });
            }
            else  // If another browser, return 0
            {
                if (className.split(' ').includes("flex-column")) {
                    item.classList.remove("flex-column");
                }

                var valueSpaces = document.getElementsByClassName("flexColumn");
                Array.from(valueSpaces).forEach(function (itemSpace) {
                    var clsName = itemSpace.className;

                    if (!clsName.split(' ').includes("tz-div-space")) {
                        itemSpace.classList.add("tz-div-space");
                    }
                });
            }
        }
        $('.tz-div-space:not(:first-child):not(:last-child)').css("margin-left", "20vh");
        $('.tz-div-space:not(:first-child):not(:last-child)').css("margin-right", "20vh");
        $('.tz-div-space:not(:first-child):not(:last-child)').css("margin-top", "");
        $('.tz-div-space:not(:first-child):not(:last-child)').css("margin-bottom", "");

        $('.tz-div-space-icontext:not(:first-child):not(:last-child)').css("margin-left", "20vh");
        $('.tz-div-space-icontext:not(:first-child):not(:last-child)').css("margin-right", "20vh");
        $('.tz-div-space-icontext:not(:first-child):not(:last-child)').css("margin-top", "");
        $('.tz-div-space-icontext:not(:first-child):not(:last-child)').css("margin-bottom", "");

        $('.tz-max-div-text').css("max-width", "30vh");
        $('.tz-max-div-text').css("width", "30vh");

        $('.tz-email-foot').css("width", "30vh");
    }
}