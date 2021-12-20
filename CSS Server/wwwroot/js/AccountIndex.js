function pencilOnClick(event, userId) {
    location.href = "/Account/Update/" + userId;
}

function addButtonOnClick(event) {
    location.href = "/Account/Register";
}

//Add on click on the addButton
var addButton = document.querySelector(".addButton");
if (addButton != null) {
    addButton.addEventListener("click", addButtonOnClick);
}

function showSnackbar(time) {
    var snackbar = document.getElementById("snackbar");
    snackbar.className = "show";
    setTimeout(function () { snackbar.className = snackbar.className.replace("show", ""); }, time);
}

//This function will be executed when the trashbin icon is clicked.
function trashbinOnClick(event, userRow, userId) {
    if (confirm("Are you sure you want to delete the user?")) {
        var url = "/Account/Delete/" + userId;
        var xhr = new XMLHttpRequest();
        xhr.open("DELETE", url);

        //define a function for letting the user know that the user is deleted succesfully (or not).
        xhr.onload = function () {
            var snackbar = document.getElementById("snackbar");
            if (xhr.status == 200) {
                snackbar.innerText = "User was deleted succesfully!"
                userRow.remove();
            } else {
                snackbar.innerText = "User was not deleted succesfully!"
            }
            showSnackbar(3000);
        };

        xhr.send(null);
    }
}

//Get all userrows from the table.
var userRows = document.querySelectorAll("tbody > tr");
Array.from(userRows).forEach(function (userRow) {
    //Get user id by getting the innerText from the first td.
    var userId = userRow.querySelector("td").innerText;

    //Get the trashbin icon and add an eventlistener to it.
    userRow.querySelector(".trashbin").addEventListener("click", trashbinOnClick.bind(null, event, userRow, userId));

    //Get the pencil icon and add an eventlistener to it.
    userRow.querySelector(".pencil").addEventListener("click", pencilOnClick.bind(null, event, userId));
});