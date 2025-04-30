document.addEventListener('DOMContentLoaded', () => {
    if (!getCookie("cookieConsent")) {
        showCookieModal();
    }
});
function openCookieConsentBanner() {
    showCookieModal();
}

function showCookieModal() {
    const modal = document.getElementById('cookieModal');
    if (modal) modal.classList.add("show");
}

function getCookie(name) {
    const nameEQ = name + "=";
    const cookies = document.cookie.split(';');
    for (let cookie of cookies) {
        cookie = cookie.trim();
        if (cookie.indexOf(nameEQ) === 0) {
            return decodeURIComponent(cookie.substring(nameEQ.length));
        }
    }
    return null;
}

function setCookie(name, value, days) {
    let expires = "";
    if (days) {
        const date = new Date();
        date.setTime(date.getTime() + days * 24 * 60 * 60 * 1000);
        expires = "; expires=" + date.toUTCString();
    }

    const encodedValue = encodeURIComponent(value || "");
    document.cookie = `${name}=${encodedValue}${expires}; path=/; SameSite=Lax`;
}

async function acceptAll() {
    const consent = {
        essential: true,
        functional: true,
        analytics: true,
        marketing: true
    };

    setCookie("cookieConsent", JSON.stringify(consent), 365);
    await handleConsent(consent);
    hideCookieModal();
}

async function acceptSelected() {
    const form = document.getElementById("cookieConsentForm");
    const formData = new FormData(form);

    const consent = {
        essential: true,
        functional: formData.get("functional") === "on",
        analytics: formData.get("analytics") === "on",
        marketing: formData.get("marketing") === "on"
    };

    setCookie("cookieConsent", JSON.stringify(consent), 365);
    await handleConsent(consent);
    hideCookieModal();
}

function hideCookieModal() {
    const modal = document.getElementById('cookieModal');
    if (modal) modal.classList.remove("show");
}

async function handleConsent(consent) {
    await fetch("/Cookies/SetCookies", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(consent)
    });
}
