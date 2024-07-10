document.addEventListener("DOMContentLoaded", function () {
    const searchInput = document.querySelector(".search-txt");
    const searchButton = document.querySelector(".search-btn");

    searchButton.addEventListener("click", function (event) {
        if (event.target.closest(".search-area")) {
            searchButton.classList.add("active");
        } else {
            if (!searchInput.value.trim()) {
                searchButton.classList.remove("active");
            }
        }
    });

    searchInput.addEventListener("focus", function () {
        searchButton.classList.add("active");
    });

    searchInput.addEventListener("blur", function () {
        if (!searchInput.value.trim()) {
            searchButton.classList.remove("active");
        }
    });
});