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