var CreateRoomBtn = document.getElementById('create-room-btn');
var CreateRoomModal = document.getElementById('create-room-modal');

CreateRoomBtn.addEventListener('click', function () {
    CreateRoomModal.classList.add('active')
});

var closeModal = function() {
    CreateRoomModal.classList.remove('active');
}