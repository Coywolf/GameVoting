var ViewModel = function(data) {
    var self = this;

    self.Events = ko.observableArray($.map(data.events, function (e) {
        return new EventModel(e);
    }));
    self.NewEvent = ko.observable(new EventModel({}));

    self.CreateEvent = function() {
        $.ajax({
            url: '/Event/CreateEvent',
            type: 'POST',
            data: {
                data: ko.toJSON(self.NewEvent())
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
    self.StartDate = ko.observable(data.StartDate);
    self.EndDate = ko.observable(data.EndDate);
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