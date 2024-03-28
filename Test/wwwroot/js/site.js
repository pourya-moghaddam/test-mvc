

document.getElementsByTagName("button")[0].addEventListener("click", function () {
    var elements = document.getElementsByClassName("name")
    var count = elements.length
    var newNameLabel = document.createElement("label")
    // var newSurnameLabel = document.createElement("label")
    var newNameInput = document.createElement("input")
    // var newSurnameInput = document.createElement("input")
    newNameLabel.setAttribute("for", "Fields_" + count + "__Name")
    // newSurnameLabel.setAttribute("for", "Fields_" + count + "__Surname")
    newNameLabel.innerHTML = "Name"
    // newSurnameLabel.innerHTML = "Surname"
    newNameInput.setAttribute("id", "Fields_" + count + "__Name")
    // newSurnameInput.setAttribute("id", "People_" + count + "__Surname")
    newNameInput.setAttribute("class", "name")
    // newSurnameInput.setAttribute("class", "surname")
    newNameInput.setAttribute("name", "Fields[" + count + "].Name")
    // newSurnameInput.setAttribute("name", "People[" + count + "].Surname")
    newNameInput.setAttribute("type", "text")
    // newSurnameInput.setAttribute("type", "text")
    document.getElementsByTagName("form")[0].append(newNameLabel)
    document.getElementsByTagName("form")[0].append(newNameInput)
    document.getElementsByTagName("form")[0].append(document.createElement("br"))
    // document.getElementsByTagName("form")[0].append(newSurnameLabel)
    // document.getElementsByTagName("form")[0].append(newSurnameInput)
    // document.getElementsByTagName("form")[0].append(document.createElement("br"))
})

$.ajax({
    beforeSend: function(xhr) {
        xhr.setRequestHeader("X-XSRF-TOKEN",
            $('input:hidden[name="__RequestVerificationToken"]').val());
    }
})
