alert("sadads");

document.addEventListener('DOMContentLoaded', function () {
    alert("sadads");
    const list = document.querySelector('.list');
    const items = document.querySelectorAll('.blocks_item');

    list.addEventListener('click', function (event) {
        const targetId = event.target.dataset.id;
        console.log('work');
        items.forEach(function (item) {
            if (item.classList.contains(targetId)) {
                console.log(targetId);
                console.log(item);
                item.style.display = 'flex';
            } else {
                console.log("dasd");
                item.style.display = 'none';
            }
        });
    });
});