//Get all inputboxes
Array.from(document.querySelectorAll(".inputBox")).forEach(function (inputBox) {
    //Foreach input box add the keydown event listener.
    inputBox.querySelector("input").addEventListener("keydown", function () {
        //When the keydown event happens, the validationerrors in the inputBox will be removed.
        Array.from(inputBox.querySelectorAll(".validationError")).forEach(function (error) {
            error.remove();
        });
    });
});
