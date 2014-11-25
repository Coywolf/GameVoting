var ViewModel = function () {
    var self = this;

    self.Events = ko.observableArray();
    self.NewEvent = ko.observable(new EventModel({}));
    self.NewEventInit = function (elements) {
        var typeElement = $(elements).find('#type-select').addBack('#type-select').first();
        typeElement.chosen({
            width: "100%",
            placeholder_text_single: "Event Type"
        });
        
        var memberElement = $(elements).find('#member-select').addBack('#member-select').first();
        memberElement.chosen({
            width: "100%",
            placeholder_text_multiple: "Members (Optional)"
        });
    };

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
                window.location.href = window.location.origin + '/Event/View/' + data.EventId;
            }
        });
    };
};

var EventModel = function(data) {
    var self = this;

    self.EventId = data.EventId;
    self.Name = ko.observable(data.Name);
    self.TypeId = ko.observable(data.TypeId);
    self.IsPrivate = ko.observable(data.IsPrivate);
    self.StartDate = ko.observable(data.StartDate);
    self.EndDate = ko.observable(data.EndDate);

    self.url = "/Event/View/" + self.EventId;
};

$(document).ready(function () {
    //initialize with an empty model
    var model = new ViewModel();
    ko.applyBindings(model);
    
    $.ajax({
        url: '/Event/GetEvents',
        dataType: 'json',
        success: function (data) {
            model.Events($.map(data.events, function(e) {
                return new EventModel(e);
            }));
        }
    });
});