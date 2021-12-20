function showSnackbar(time) {
    var snackbar = document.getElementById("snackbar");
    snackbar.className = "show";
    setTimeout(function () { snackbar.className = snackbar.className.replace("show", ""); }, time);
}

//This function will be executed when the trashbin icon is clicked.
function trashbinOnClick(event, cameraBlock, cameraId) {

    if (confirm("Are you sure you want to delete the camera?")) {
        var url = "/Camera/Delete/" + cameraId;
        var xhr = new XMLHttpRequest();
        xhr.open("DELETE", url);

        //define a function for letting the user know that the camera is deleted succesfully (or not).
        xhr.onload = function () {
            var snackbar = document.getElementById("snackbar");
            if (xhr.status == 200) {
                snackbar.innerText = "Camera was deleted succesfully!"
                cameraBlock.remove();
            } else {
                snackbar.innerText = "Camera was not deleted succesfully!"
            }
            showSnackbar(3000);
        };

        xhr.send(null);
    }
}

function pecilOnClick(event, cameraId) {
    location.href = "/Camera/Update/" + cameraId;
}

function addButtonOnClick(event) {
    location.href = "/Camera/Register";
}

//Add on click on the addButton
var addButton = document.querySelector(".addButton");
if (addButton != null) {
    addButton.addEventListener("click", addButtonOnClick);
}

//Get all camera blocks
var cameraBlocks = document.querySelectorAll(".single-camera-block");

Array.from(cameraBlocks).forEach(function (cameraBlock) {

    //Get the id of the presented camera
    var cameraId = cameraBlock.getAttribute("cameraId");

    //get the trashbin and pencil icons
    var trashbin = cameraBlock.querySelector(".trashbin");
    var pencil = cameraBlock.querySelector(".pencil");

    //add an onclick action to the trashbin and pencil icons
    trashbin.addEventListener("click", trashbinOnClick.bind(null, event, cameraBlock, cameraId));
    pencil.addEventListener("click", pecilOnClick.bind(null, event, cameraId));

    if (cameraBlock.getAttribute("online") == "True") {
        cameraBlock.addEventListener("click", function (event) {

            //prevent the onclick action when the trashbin button is clicked.
            if (event.target == trashbin || event.target == pencil) {
                return;
            }
            location.href = "/Camera/Footage/" + cameraId;
        });
    }
});