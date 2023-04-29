function GotoControllerAsync(url, type, data, includeAntiforgeToken, returnType, successCallback) {

    const promiseAsync = new Promise((resolve, reject) => {
        resolve('resolve');
    });

    promiseAsync.then((resolve) => {

        ProcessAsync(url, type, data, includeAntiforgeToken, returnType)
            .then((response) => {

                if (typeof successCallback != 'undefined' && typeof successCallback == 'function') {
                    successCallback(response);
                }
            });
    });
}

async function ProcessAsync(url, type, obj, includeAntiforgeToken, returnType) {
    return await $.ajax({
        type: type,
        url: url,
        contentType: includeAntiforgeToken ? 'application/json; charset=utf-8' : 'application/x-www-form-urlencoded; charset-UTF-8',
        data: includeAntiforgeToken ? JSON.stringify(obj) : obj,
        dataType: returnType,
        async: true,
        beforeSend: function (jqXHR, settings) {

            if (includeAntiforgeToken)
                jqXHR.setRequestHeader('RequestVerificationToken', $('.AntiForge' + ' input').val());
        }
    });
}

function submitForm(formId, link, objData, callback) {
    //let formDataToSubmit = new FormData();
    let formData = $('#' + formId).serializeArray();
    let dataSource = {};
    $(formData).each(function (index, obj) {

        // if value is "on" set to "1"
        //formDataToSubmit.append(obj.name, obj.value == "on" ? 1 : obj.value);
        dataSource[obj.name] = obj.value == "on" ? true : obj.value;
    });

    console.log('datasource: ', dataSource);

    GotoControllerAsync(link
        , 'POST'
        , dataSource
        , false
        , 'json'
        , function (response) {

            if (callback != null && typeof callback == 'function') {
                callback(response);
            }
        });
}

function submitFormWithFiles(formId, link, fileId, callback) {
    // append file input
    let formData = new FormData();
    formData.append(fileId, $("#" + fileId)[0].files[0]);

    // append other input data
    let serialized = $('#' + formId).serializeArray();
    $(serialized).each(function (index, obj) {
        formData.append(obj.name, obj.value == "on" ? true : obj.value);
    });

    $.ajax({
        type: 'POST',
        url: link,
        data: formData,
        dataType: 'json',
        contentType: false,
        processData: false,
        success: function (response) {

            if (callback != null && typeof callback == 'function') {
                callback(response);
            }
        }
    });
}

function refreshTable(tableId) {
    if ($('#' + tableId).length > 0)
        $('#' + tableId).DataTable().ajax.reload();    
}

function onclickRedirectToReply(model) {
    let url = '';

    switch (model.NotificationQueryTypeID) {
        case 1:
            url = '/QueryManuscript/Reply?id=' + model.QueryReplyID
                + '&queryid=' + model.QueryID
                + '&v=' + false
                + '&u=' + model.UserAccessID
                + '&fi=' + true
                + '&e=' + model.EmployeeID
                + '&ni=' + model.ID;
            break
        case 2:
            url = '/QueryCoversheet/Reply?id=' + model.QueryReplyID
                + '&queryid=' + model.QueryID
                + '&v=' + false
                + '&u=' + model.UserAccessID
                + '&fi=' + true
                + '&e=' + model.EmployeeID
                + '&ni=' + model.ID;
            break
        case 3:
            url = '/QuerySTP/Reply?id=' + model.QueryReplyID
                + '&queryid=' + model.QueryID
                + '&v=' + false
                + '&u=' + model.UserAccessID
                + '&fi=' + true
                + '&e=' + model.EmployeeID
                + '&ni=' + model.ID;
            break
    }

    window.location.href = url;
}

function validateForm(formId) {
    // true/false (validity checking)
    let isValid = $('#' + formId)[0].checkValidity();
    let inputs = $('#' + formId + ' :input');

    if (!isValid) {
        inputs.each(function (i) {

            // invalid fields are highlighted in red
            if (inputs[i].id != '') {
                if (!inputs[i].checkValidity()) {
                    $('#' + inputs[i].id).css('border-color', '#e52213');
                }
            }
        });

        $('#ErrorMessage').show();

        if ($('#ErrorMessage2').length > 0) {            
            $('#ErrorMessage').hide();
            $('#ErrorMessage2').show();
        }
    }

    return isValid;
}

function removeRedError(e) {
    $('#' + e.id).css('border-color', '#ced4da');
}

function setupDefaultForm() {
    $('#ErrorMessage').hide();
    $('#ErrorMessage2').hide();
    $('.datepicker').datepicker({
        todayHighlight: true,
        format: 'yyyy-mm-dd',
        clearBtn: true,
        autoclose: true
    });
}

function setupForMultipleModal() {
    $(document).on('show.bs.modal', '.modal', function (event) {
        var zIndex = 1040 + (10 * $('.modal:visible').length);
        $(this).css('z-index', zIndex);

        setTimeout(function () {
            $('.modal-backdrop').not('.modal-stack').css('z-index', zIndex - 1).addClass('modal-stack');
        }, 0);
    });
}