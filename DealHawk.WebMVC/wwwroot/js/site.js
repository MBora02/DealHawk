function handleImageError(img, steamAppId, thumbnailUrl) {
    const placeholder = 'https://images.unsplash.com/photo-1542751371-adc38448a05e?auto=format&fit=crop&w=400&q=80';
    const currentSrc = img.src;

    if (steamAppId && steamAppId !== '0' && steamAppId !== 'null' && steamAppId !== '') {

        if (currentSrc.indexOf('library_600x900.jpg') !== -1) {
            const nextSrc = 'https://cdn.akamai.steamstatic.com/steam/apps/' + steamAppId + '/header.jpg';
            img.src = nextSrc;
            updateBlurBg(img, nextSrc);
            return;
        }

        if (currentSrc.indexOf('header.jpg') !== -1) {
            const nextSrc = thumbnailUrl || placeholder;
            img.src = nextSrc;
            updateBlurBg(img, nextSrc);
            return;
        }
    }

    if (currentSrc !== placeholder) {
        img.src = placeholder;
        updateBlurBg(img, placeholder);
    }

    img.onerror = null;
}

function updateBlurBg(img, src) {
    if (img.previousElementSibling && img.previousElementSibling.classList.contains('game-img-blur-bg')) {
        img.previousElementSibling.style.backgroundImage = "url('" + src + "')";
    }
}
