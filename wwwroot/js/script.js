function Register(em, pass, reppass, dt) {
    if (pass === reppass) {
        $.ajax({
            type: "GET",
            url: `/api/Users/CheckAvailableEmail/${em}`,
            contentType: "application/json",
            dataType: "json",
            success: function (res) {
                if (res.value === false) {
                    Swal.fire({
                        position: "center",
                        icon: "error",
                        title: "Такой email уже зарегистрирован в системе!",
                        showConfirmButton: false,
                        timer: 1500
                    });
                }
                else {
                    $.ajax({
                        type: "POST",
                        url: '/api/Users/Create',
                        data: JSON.stringify({
                            "id": 0,
                            "email": em,
                            "password": pass,
                            "firstName": "",
                            "lastName": "",
                            "middleName": "",
                            "createionDate": ConvertDateTime(dt),
                            "roleId": 1,
                            "isDeleted": false
                        }),
                        contentType: "application/json",
                        dataType: "json",
                        success: function () {
                            Swal.fire({
                                position: "center",
                                icon: "success",
                                title: "Успешно!",
                                showConfirmButton: false,
                                timer: 1500
                            }).then(() => {
                                window.location.href = '/Logon/Index';
                            });
                        },
                        error: function () {
                            Swal.fire({
                                position: "center",
                                icon: "error",
                                title: "Произошла ошибка, повторите попытку позже!",
                                showConfirmButton: false,
                                timer: 1500
                            });
                        }
                    })
                }
            },
            error: function () {
                Swal.fire({
                    position: "center",
                    icon: "error",
                    title: "Произошла ошибка, повторите попытку позже!",
                    showConfirmButton: false,
                    timer: 1500
                });
            }
        })
    }
    else {
        Swal.fire({
            position: "center",
            icon: "error",
            title: "Пароли различаются!",
            showConfirmButton: false,
            timer: 1500
        });
    }
};

function ConvertDateTime(dt) {
    // Исходная строка времени
    let originalDateString = dt;
    // Разбиваем строку на компоненты даты и времени
    let parts = originalDateString.split(" ");
    let dateParts = parts[0].split("/");
    let timeParts = parts[1].split(":");
    // Создаем новый объект Date
    let newDate = new Date(Date.UTC(dateParts[2], dateParts[0] - 1, dateParts[1], timeParts[0], timeParts[1], timeParts[2]));
    // Вычитаем 3 часа
    newDate.setHours(newDate.getHours() - 3);
    // Возвращаем новую дату в формате ISO
    return newDate.toISOString();
};

function Login(em, pass) {
    if (em !== "" && pass !== "") {
        $.ajax({
            type: "GET",
            url: `/api/Users/GetByEmailAndPassword/${em}/${pass}`,
            contentType: "application/json",
            dataType: "json",
            success: function (user) {
                Swal.fire({
                    position: "center",
                    icon: "success",
                    title: "Авторизация прошла успешно!",
                    showConfirmButton: false,
                    timer: 1500
                }).then(() => { 
                    if (user.value.roleId === 1 || user.value.roleId === 2) {

                        window.location.href = '/Home/Index';
                    }
                    else if (user.value.roleId === 3) {
                        window.location.href = '/Administration/Index';
                    }
                });
            },
            error: function (msg) {
                Swal.fire({
                    position: "center",
                    icon: "error",
                    title: "Произошла ошибка, повторите попытку позже!",
                    showConfirmButton: false,
                    timer: 1500
                });
            }
        })
    }
    else {
        Swal.fire({
            position: "center",
            icon: "error",
            title: "Произошла ошибка, повторите попытку позже!",
            showConfirmButton: false,
            timer: 1500
        });
    }
};

function Logoff() {
    $.get(`/Logon/Logout`).then(() => {
        window.location.href = '/Home/Index';
    })
};

function RegToEvent(iduser, idevent) {
    $.ajax({
        type: "POST",
        url: `/api/UserEvent/EventRegistration/${iduser}/${idevent}`,
        contentType: "application/json",
        dataType: "json",
        success: function () {
            Swal.fire({
                position: "center",
                icon: "success",
                title: "Регистрация на мероприятие прошло успешно!",
                showConfirmButton: false,
                timer: 1500
            }).then(() => {
                location.reload();
            });
        },
        error: function (msg) {
            Swal.fire({
                position: "center",
                icon: "success",
                title: "Произошла неожиданная ошибка, повторите попытку позже!",
                showConfirmButton: false,
                timer: 1500
            });
        }
    });
};

function loadPartialView(partialViewName) {
    $.get(`/Profile/Get${partialViewName}Partial`, function (data) {
        $("#partialViewContainer").html(data);
    }).then(() => {
        if (partialViewName === "LogonSetting") {
            document.getElementById(`label${partialViewName}`).style.color = '#7848F4';
            document.getElementById(`labelPersonalSetting`).style.color = '#000000';
            document.getElementById(`labelUndertakenActivities`).style.color = '#000000';
        }
        else if (partialViewName === "PersonalSetting") {
            document.getElementById(`label${partialViewName}`).style.color = '#7848F4';
            document.getElementById(`labelLogonSetting`).style.color = '#000000';
            document.getElementById(`labelUndertakenActivities`).style.color = '#000000';
        }
        else if (partialViewName === "UndertakenActivities") {
            document.getElementById(`label${partialViewName}`).style.color = '#7848F4';
            document.getElementById(`labelLogonSetting`).style.color = '#000000';
            document.getElementById(`labelPersonalSetting`).style.color = '#000000';
        }
        else if (partialViewName === "CommonSetting") {
            document.getElementById(`label${partialViewName}`).style.color = '#7848F4';
            document.getElementById(`labelLocationSetting`).style.color = '#000000';
        }
        else if (partialViewName === "LocationSetting") {
            document.getElementById(`label${partialViewName}`).style.color = '#7848F4';
            document.getElementById(`labelCommonSetting`).style.color = '#000000';
        }
    });
};

function UpdatePersonalSetting(userid, lastname, firstname, middlename) {
    $.ajax({
        type: "PUT",
        url: `/api/Users/UpdatePersonal/${userid}/${lastname}/${firstname}/${middlename}`,
        contentType: "application/json",
        dataType: "json",
        success: function () {
            Swal.fire({
                position: "center",
                icon: "success",
                title: "Данные успешно обновлены!",
                showConfirmButton: false,
                timer: 1500
            }).then(() => {
                location.reload();
            });
        },
        error: function (msg) {
            Swal.fire({
                position: "center",
                icon: "error",
                title: "Произошла ошибка, повторите попытку позже!",
                showConfirmButton: false,
                timer: 1500
            });
        }
    });
};

function UpdateLogonSetting(userid, pass, reppas) {
    if (pass === reppas) {
        $.ajax({
            type: "PUT",
            url: `/api/Users/UpdateLogon/${userid}/${pass}`,
            contentType: "application/json",
            dataType: "json",
            success: function () { 
                Swal.fire({
                    position: "center",
                    icon: "success",
                    title: "Данные успешно обновлены!",
                    showConfirmButton: false,
                    timer: 1500
                }).then(() => {
                    location.reload();
                });
            },
            error: function (msg) {
                Swal.fire({
                    position: "center",
                    icon: "error",
                    title: "Произошла ошибка, повторите попытку позже!",
                    showConfirmButton: false,
                    timer: 1500
                });
            }
        });
    }
    else {
        Swal.fire({
            position: "center",
            icon: "error",
            title: "Пароли отличаются!",
            showConfirmButton: false,
            timer: 1500
        });
    }
};

function saveSchoolPhoto(schoolId, photoFile, name, desc) {
    let formData = new FormData();
    formData.append('photo', photoFile);

    if (photoFile === undefined && (name !== "" && desc !== "")) {
        $.ajax({
            type: "PUT",
            url: `/api/Academy/UpdateInfo/${schoolId}/${name}/${desc}`,
            contentType: "application/json",
            dataType: "json",
            success: function () {
                Swal.fire({
                    position: "center",
                    icon: "success",
                    title: "Наименование и описание о школе успешно обновлены!",
                    showConfirmButton: false,
                    timer: 1500
                }).then(() => {
                    location.reload()
                });
            },
            error: function () {
                Swal.fire({
                    position: "center",
                    icon: "error",
                    title: "Произошла ошибка, повторите попытку позже!",
                    showConfirmButton: false,
                    timer: 1500
                });
            }
        });
    }
    else if (photoFile !== undefined && (name !== "" && desc !== "")) {
        fetch(`/api/Academy/AddSchoolPhoto/${schoolId}`, {
            method: 'POST',
            body: formData
        }).then(() => {
            $.ajax({
                type: "PUT",
                url: `/api/Academy/UpdateInfo/${schoolId}/${name}/${desc}`,
                contentType: "application/json",
                dataType: "json",
            });
        }).then(() => {
            Swal.fire({
                position: "center",
                icon: "success",
                title: "Информация о школе успешно обновлена!",
                showConfirmButton: false,
                timer: 1500
            }).then(() => {
                location.reload()
            });
        });
    }
}

function openModalCreateEvent(options) {
    const schoolid = options.schoolid;
    const url = options.url;
    const modal = $('#modal');

    $.ajax({
        type: 'GET',
        url: url,
        data: { "schoolid": schoolid },
        success: function (response) {
            $('.modal-dialog');
            modal.find(".modal-body").html(response);
            modal.modal('show');
        },
        failure: function () {
            modal.modal('hide');
        }
    }).then(() => {
        let containerElement = document.querySelector('#modal');
        containerElement.setAttribute('style', 'display: flex !important');
    });
};

function openModalAddReviewToEvent(options) {
    const eventid = options.eventId;
    const url = options.url;
    const modal = $('#modal');

    $.ajax({
        type: 'GET',
        url: url,
        data: { "eventid": eventid },
        success: function (response) {
            $('.modal-dialog');
            modal.find(".modal-body").html(response);
            modal.modal('show');
        },
        failure: function () {
            modal.modal('hide');
        }
    }).then(() => {
        let containerElement = document.querySelector('#modal');
        containerElement.setAttribute('style', 'display: flex !important');
    });
};

function openModalCreateSchool(options) {
    const url = options.url;
    const userid = options.userId;
    const modal = $('#modal');

    $.ajax({
        type: 'GET',
        url: url,
        data: { "userid": userid },
        success: function (response) {
            $('.modal-dialog');
            modal.find(".modal-body").html(response);
            modal.modal('show');
        },
        failure: function () {
            modal.modal('hide');
        }
    }).then(() => {
        let containerElement = document.querySelector('#modal');
        containerElement.setAttribute('style', 'display: flex !important');
    });
};

function openModalEditEvent(options) {
    const url = options.url;
    const eventid = options.eventid;
    const modal = $('#modal');

    $.ajax({
        type: 'GET',
        url: url,
        data: { "eventid": eventid },
        success: function (response) {
            $('.modal-dialog');
            modal.find(".modal-body").html(response);
            modal.modal('show');
        },
        failure: function () {
            modal.modal('hide');
        }
    }).then(() => {
        let containerElement = document.querySelector('#modal');
        containerElement.setAttribute('style', 'display: flex !important');
    });
};

function AddActivity(name, desc, datetime, capacity, evtype, photo, schoolid) {
    let formData = new FormData();
    formData.append('photo', photo);

    var dt = ConvertDateTime2(datetime);

    fetch(`/api/Events/Create/${schoolid}/${name}/${desc}/${dt}/${capacity}/${evtype}`, {
        method: 'POST',
        body: formData
    }).then(() => {
        Swal.fire({
            position: "center",
            icon: "success",
            title: "Мероприятие успешно создано!",
            showConfirmButton: false,
            timer: 1500
        }).then(() => {
            location.reload();
        });
    })
};

function ConvertDateTime2(dateTimeString) {
    // Разбор и парсинг строки даты и времени с помощью Luxon
    let dateTime = luxon.DateTime.fromISO(dateTimeString);

    // Форматирование даты и времени в нужный формат
    let formattedDateTime = dateTime.toFormat("yyyy-MM-dd HH:mm:ssZ");

    return formattedDateTime;
};

function SetRegionId(regionid) {
    $.get(`/Profile/Index?regionid=${regionid}`).then(_ => {
        window.location.href = `/Profile/Index?regionid=${regionid}`;
    }).then(_ => { 
        loadPartialView('LocationSetting');
    });
};

function saveSchoolLocation(schoolid, cityid, street, house) {
    $.ajax({
        type: "PUT",
        url: `/api/Academy/UpdateLocation/${schoolid}/${cityid}/${street}/${house}`,
        contentType: "application/json",
        dataType: "json",
        success: function () {
            Swal.fire({
                position: "center",
                icon: "success",
                title: "Местоположение школы успешно обновлено!",
                showConfirmButton: false,
                timer: 1500
            }).then(() => {
                location.reload();
            });
        },
        error: function () {
            Swal.fire({
                position: "center",
                icon: "error",
                title: "Произошла ошибка, повторите попытку позже!",
                showConfirmButton: false,
                timer: 1500
            });
        }
    });
};

function RedirectTo(link) {
    window.location.href = link;
};

function SearchEvents() {
    let type = document.getElementById('evtype').value;
    let city = document.getElementById('evcity').value;
    let date = document.getElementById('evdate').value;

    $.get(`/Activities/Index?type=${type}&cityid=${city}&date=${date}`).then(_ => {
        window.location.href = `/Activities/Index?type=${type}&cityid=${city}&date=${date}`;
    });
};

function RedirectToMap(eventid) {
    window.location.href = `/Activities/Activity?activityid=${eventid}#map`;
};

function SetEventReview(userid, eventid, desc) {
    var selectedRating = document.querySelector('input[name="rating"]:checked').value;
    $.ajax({
        type: "POST", 
        url: `/api/Review/AddEventReview/${userid}/${eventid}/${selectedRating}/${desc}`,
        contentType: "application/json",
        dataType: "json",
        success: function () {
            Swal.fire({
                position: "center",
                icon: "success",
                title: "Отзыв на мероприятие успешно добавлен!",
                showConfirmButton: false,
                timer: 1500
            }).then(() => {
                location.reload();
            });
        },
        error: function () {
            Swal.fire({
                position: "center",
                icon: "error",
                title: "Произошла ошибка, повторите попытку позже!",
                showConfirmButton: false,
                timer: 1500
            });
        }
    });
};

function switchBetweenEditing(into, toswitch) {
    var to = document.getElementById(toswitch);
    to.style.display = 'block';

    var from = document.getElementById(into);
    from.style.display = 'none';
};


function BlockUser(userid) {
    $.ajax({
        type: "POST",
        url: `/api/Users/Block/${userid}`,
        contentType: "application/json",
        dataType: "json",
        success: function (user) {
            Swal.fire({
                position: "center",
                icon: "success",
                title: `Пользователь ${user.value.email} успешно заблокирован!`,
                showConfirmButton: false,
                timer: 1500
            }).then(() => {
                location.reload();
            });
        },
        error: function () {
            Swal.fire({
                position: "center",
                icon: "error",
                title: "Произошла ошибка, повторите попытку позже!",
                showConfirmButton: false,
                timer: 1500
            });
        }
    });
};

function UnblockUser(userid) {
    $.ajax({
        type: "POST",
        url: `/api/Users/Unblock/${userid}`,
        contentType: "application/json",
        dataType: "json",
        success: function (user) {
            Swal.fire({
                position: "center",
                icon: "success",
                title: `Пользователь ${user.value.email} успешно разблокирован!`,
                showConfirmButton: false,
                timer: 1500
            }).then(() => {
                location.reload();
            });
        },
        error: function () {
            Swal.fire({
                position: "center",
                icon: "error",
                title: "Произошла ошибка, повторите попытку позже!",
                showConfirmButton: false,
                timer: 1500
            });
        }
    });
};

function BlockEvent(eventid) {
    $.ajax({
        type: "POST",
        url: `/api/Events/Block/${eventid}`,
        contentType: "application/json",
        dataType: "json",
        success: function (event) {
            Swal.fire({
                position: "center",
                icon: "success",
                title: `Мероприятие ${event.value.name} успешно удалено!`,
                showConfirmButton: false,
                timer: 1500
            }).then(() => {
                location.reload();
            });
        },
        error: function () {
            Swal.fire({
                position: "center",
                icon: "error",
                title: "Произошла ошибка, повторите попытку позже!",
                showConfirmButton: false,
                timer: 1500
            });
        }
    });
};

function UnblockEvent(eventid) {
    $.ajax({
        type: "POST",
        url: `/api/Events/Unblock/${eventid}`,
        contentType: "application/json",
        dataType: "json",
        success: function (event) {
            Swal.fire({
                position: "center",
                icon: "success",
                title: `Мероприятие ${event.value.name} успешно восстановлено!`,
                showConfirmButton: false,
                timer: 1500
            }).then(() => {
                location.reload();
            });
        },
        error: function () {
            Swal.fire({
                position: "center",
                icon: "error",
                title: "Произошла ошибка, повторите попытку позже!",
                showConfirmButton: false,
                timer: 1500
            });
        }
    });
};

function BlockSchool(schoolid) {
    $.ajax({
        type: "PUT",
        url: `/api/Academy/Block/${schoolid}`,
        contentType: "application/json",
        dataType: "json",
        success: function (school) {
            Swal.fire({
                position: "center",
                icon: "success",
                title: `Школа ${school.value.name} успешно заблокирована!`,
                showConfirmButton: false,
                timer: 1500
            }).then(() => {
                location.reload();
            });
        },
        error: function () {
            Swal.fire({
                position: "center",
                icon: "error",
                title: "Произошла ошибка, повторите попытку позже!",
                showConfirmButton: false,
                timer: 1500
            });
        }
    });
};

function UnblockSchool(schoolid) {
    $.ajax({
        type: "PUT",
        url: `/api/Academy/Unblock/${schoolid}`,
        contentType: "application/json",
        dataType: "json",
        success: function (school) {
            Swal.fire({
                position: "center",
                icon: "success",
                title: `Школа ${school.value.name} успешно восстановлена!`,
                showConfirmButton: false,
                timer: 1500
            }).then(() => {
                location.reload();
            });
        },
        error: function () {
            Swal.fire({
                position: "center",
                icon: "error",
                title: "Произошла ошибка, повторите попытку позже!",
                showConfirmButton: false,
                timer: 1500
            });
        }
    });
};

function DeleteReview(reviewid) {
    $.ajax({
        type: "DELETE",
        url: `/api/Review/Delete/${reviewid}`,
        contentType: "application/json",
        dataType: "json",
        success: function () {
            Swal.fire({
                position: "center",
                icon: "success",
                title: `Отзыв успешно удален!`,
                showConfirmButton: false,
                timer: 1500
            }).then(() => {
                location.reload();
            });
        },
        error: function () {
            Swal.fire({
                position: "center",
                icon: "error",
                title: "Произошла ошибка, повторите попытку позже!",
                showConfirmButton: false,
                timer: 1500
            });
        }
    });
};

function switchBetweenAdmitions(into1, into2, into3, toswitch) {
    var to = document.getElementById(toswitch);
    to.style.display = 'block';
    var tolabel = document.getElementById("label" + toswitch);
    tolabel.style.color = "#7848F4";

    var from1 = document.getElementById(into1);
    from1.style.display = 'none';
    var from1label = document.getElementById("label" + into1);
    from1label.style.color = "#000000";

    var from2 = document.getElementById(into2);
    from2.style.display = 'none';
    var from2label = document.getElementById("label" + into2);
    from2label.style.color = "#000000";

    var from3 = document.getElementById(into3);
    from3.style.display = 'none';
    var from3label = document.getElementById("label" + into3);
    from3label.style.color = "#000000";
};

function AddSchool(name, email, desc, photo) {
    let formData = new FormData();
    formData.append('photo', photo);

    fetch(`/api/Academy/Create/${name}/${email}/${desc}`, {
        method: 'POST',
        body: formData
    }).then(() => {
        Swal.fire({
            position: "center",
            icon: "success",
            title: "Школа успешно создана!",
            showConfirmButton: false,
            timer: 1500
        }).then(() => {
            location.reload();
        });
    })
};

function SendMailToRecovery(email) {
    $.ajax({
        type: "POST",
        url: `/api/Users/SendCodeByEmail/${email}`,
        contentType: "application/json",
        dataType: "json",
        success: function () {
            Swal.fire({
                position: "center",
                icon: "success",
                title: `На ваш электронный адрес ${email} отправлен код восстановления пароля!`,
                showConfirmButton: false,
                timer: 1500
            }).then(() => {
                var elem = document.getElementById("aftersendcode");
                elem.style.display = 'block';

                var elememail = document.getElementById("email");
                elememail.disabled = true;
                var elembtn = document.getElementById("btncheckpass");
                elembtn.disabled = true;
            });
        },
        error: function () {
            Swal.fire({
                position: "center",
                icon: "error",
                title: "Произошла ошибка, повторите попытку позже!",
                showConfirmButton: false,
                timer: 1500
            });
        }
    });
};

function SaveNewPassword(email, code, pass) {
    $.ajax({
        type: "POST",
        url: `/api/Users/UpdatePasswordByEmailAndCode/${email}/${code}/${pass}`,
        contentType: "application/json",
        dataType: "json",
        success: function (flag) {
            if (flag.value === true) {
                Swal.fire({
                    position: "center",
                    icon: "success",
                    title: `Пароль успешно изменен!`,
                    showConfirmButton: false,
                    timer: 1500
                }).then(() => {
                    location.reload();
                });
            }
            else if (flag.value === false) {
                Swal.fire({
                    position: "center",
                    icon: "error",
                    title: "Произошла ошибка, повторите попытку позже!",
                    showConfirmButton: false,
                    timer: 1500
                });
            }
        }
    });
};

function UpdateActivity(name, desc, dt, evtype, evid) {
    $.ajax({
        type: "PUT",
        url: `/api/Events/Update/${evid}/${name}/${desc}/${dt}/${evtype}`,
        contentType: "application/json",
        dataType: "json",
        success: function (flag) {
            if (flag.value === true) {
                Swal.fire({
                    position: "center",
                    icon: "success",
                    title: `Мероприятие успешно обновлено!`,
                    showConfirmButton: false,
                    timer: 1500
                }).then(() => {
                    location.reload();
                });
            }
            else if (flag.value === false) {
                Swal.fire({
                    position: "center",
                    icon: "error",
                    title: "Произошла ошибка, повторите попытку позже!",
                    showConfirmButton: false,
                    timer: 1500
                });
            }
        }
    });
}