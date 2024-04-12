document.getElementById("acceptCookieConsent").onclick = function() {
    document.getElementById("cookieConsentContainer").style.display = "none";
    document.cookie = "CookieConsent=true; path=/; expires=" + new Date(new Date().setFullYear(new Date().getFullYear() + 1)).toUTCString();
};