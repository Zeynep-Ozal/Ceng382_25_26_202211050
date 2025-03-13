function login() {
    const password = document.getElementById("passwordbox");
    const username = document.getElementById("usernamebox");
    const submit = document.getElementById("submitbox");

    submit.addEventListener('click', (event) => {
        event.preventDefault(); 

        if (username.value === "admin" && password.value === "admin") {
            window.location.href = "table.html"; 
        } else {
            alert("Wrong password or username");
        }
    });
}


login();
