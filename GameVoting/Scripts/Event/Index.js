var viewModel = function (data) {
    var self = this;

    self.events = ko.observableArray([1, 2]);
    self.newEvent = ko.observable(new eventModel({}));

    self.CreateEvent = function () {
        $.ajax({
            url: '',
            type: 'POST',
            data: ko.toJSON(self.newEvent()),
            dataType: 'json',
            success: function (data) {
                self.newEvent(new eventModel({}));
                $('#createEvent').collapse('hide');
            }
        });
    }
}

var eventModel = function (data) {
    var self = this;

    self.name = ko.observable(data.Name);
    self.type = ko.observable(data.Type);
    self.isPrivate = ko.observable(data.IsPrivate);
}

$(document).ready(function () {
    $.ajax({
        url: '/Event/GetEvents',
        dataType: 'json',
        success: function (data) {
            ko.applyBindings(new viewModel(data));
        }
    });
});