//Get the different passwordboxes
var passwordBox = document.querySelector(".inputBox[data-name='password']");
var retypePasswordBox = document.querySelector(".inputBox[data-name='retypePassword']");

//get the changepassword checkbox
var changePassword = document.querySelector("input[name='ChangePassword']");

//Define a function for changing the display of the password boxes based on the checkbox
function setPasswordDisplay() {
    if (changePassword.checked) {
        passwordBox.style.display = 'block';
        retypePasswordBox.style.display = 'block';
        changePassword.value = "true";
    } else {
        passwordBox.style.display = 'none';
        retypePasswordBox.style.display = 'none';
        changePassword.value = "false";
    }
}

//add an event listener on the changePassword checkbox so that fields can be hidden or showed.
changePassword.addEventListener('change', setPasswordDisplay);

//set initial display style of the password boxes based on the changedpassword prop in the model:
changePassword.checked = changePassword.value;
setPasswordDisplay();