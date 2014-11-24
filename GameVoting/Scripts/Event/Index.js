var ViewModel = function(data) {
    var self = this;

    self.Events = ko.observableArray([1, 2]);
    self.NewEvent = ko.observable(new eventModel({ }));

    self.CreateEvent = function() {
        $.ajax({
            url: '',
            type: 'POST',
            data: {
                data: ko.toJSON(self.newEvent())
            },
            dataType: 'json',
            success: function (data) {
                console.log(window.location);
                window.location.href = window.location.origin + '/Event/' + data.EventId;
            }
        });
    };
};

var EventModel = function(data) {
    var self = this;

    self.Name = ko.observable(data.Name);
    self.Type = ko.observable(data.Type);
    self.IsPrivate = ko.observable(data.IsPrivate);
};

$(document).ready(function () {
    $.ajax({
        url: '/Event/GetEvents',
        dataType: 'json',
        success: function (data) {
            ko.applyBindings(new ViewModel(data));
        }
    });
});