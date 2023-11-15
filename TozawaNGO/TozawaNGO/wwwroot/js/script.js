function logoutClick() {
    document.getElementById('logoutform').click();
}

function loginClick() {
    document.getElementById('loginform').click();
}

function FooterResized(dotnethelper) {
    function padPageElement() {
        document.getElementsByClassName("mainpage")[0].style.paddingBottom = footerElm.offsetHeight + "px";
    }
    var footerElm = document.getElementById("footer");
    new ResizeObserver(padPageElement).observe(footerElm);
}