var ViewModel = function (data) {
    var self = this;
};

$(document).ready(function () {
    var splitUrl = window.location.href.split('/');
    var eventId = splitUrl[splitUrl.length - 1];  //get the last element, which should be the id

    //make sure this is a valid number
    if (!isNaN(eventId)) {
        $.ajax({
            url: 'GetEvent',
            data: {
                eventId: eventId
            },
            dataType: 'json',
            success: function (data) {
                ko.applyBindings(new ViewModel(data));
            }
        });
    }
    else {
        //todo url parse error, redirect to index?
    }
});