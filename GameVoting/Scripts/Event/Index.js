var ViewModel = function () {
    var self = this;

    self.EventTypes = ko.observableArray();
    self.Users = ko.observableArray();
    self.OptionSets = ko.observableArray();

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
        //change event to update the Members observable
        memberElement.on('change', function(ev, params) {
            if(params.selected) {
                self.NewEvent().Members.push(params.selected);
            }
            if(params.deselected) {
                self.NewEvent().Members.remove(params.deselected);
            }
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
    self.Creator = ko.observable(data.Creator);
    self.EventType = ko.observable(data.EventType);

    self.Options = ko.observableArray(data.Options ? $.map(data.Options, function (o) {
        return new OptionModel(o);
    }) : []);
    self.Members = ko.observableArray(data.Members ? $.map(data.Members, function (m) {
        return new MemberModel(m);
    }) : []);
    self.Members.subscribe(function(newVal) {
        if(newVal.length > 0) {
            self.IsPrivate(true);
        }
        else {
            self.IsPrivate(false);
        }
    });

    self.AddOption = function () {
        self.Options.push(ko.observable(""));
    };
    self.AddOptionSet = function(optionSet) {
        for (var i = 0; i < optionSet.options.length; i++) {
            self.Options.push(ko.observable(optionSet.options[i].name));
        }
    };
    self.RemoveOption = function (option) {
        self.Options.remove(option);
    };

    self.url = "/Event/View/" + self.EventId;
};

var OptionModel = function(data) {
    var self = this;

    self.OptionId = data.OptionId;
    self.Name = ko.observable(data.Name);
};

var MemberModel = function(data) {
    var self = this;

    self.UserId = data.UserId;
    self.UserName = ko.observable(data.UserName);
};

var debug;
$(document).ready(function () {
    //initialize with an empty model
    var model = new ViewModel();
    ko.applyBindings(model);
    debug = model;
    
    $.ajax({
        url: '/Event/GetEventData',
        dataType: 'json',
        success: function (data) {
            model.EventTypes($.map(data.eventTypes, function (e) {
                return {
                    id: e.TypeId,
                    name: e.Name
                };
            }));
            model.Users($.map(data.users, function (u) {
                return {
                    id: u.UserId,
                    name: u.UserName
                };
            }));
            model.OptionSets($.map(data.optionSets, function (os) {
                return {
                    id: os.OptionSetId,
                    name: os.Name,
                    options: $.map(os.Options, function (o) {
                        return {
                            id: o.OptionId,
                            name: o.Name
                        }
                    })
                };
            }));
            model.Events($.map(data.events, function(e) {
                return new EventModel(e);
            }));
        }
    });
});