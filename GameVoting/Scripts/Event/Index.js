var ViewModel = function () {
    var self = this;

    self.EventTypes = ko.observableArray();
    self.Users = ko.observableArray();
    self.GameSets = ko.observableArray();
    
    self.EventCount = ko.observable(0);
    self.Events = ko.observableArray();
    self.NewEvent = ko.observable(new EventModel({}));
    self.TypeListInit = function (elements, data) {
        //i hate this if check, there has to be a more elegant way
        //but i need this to only happen after everything is rendered. +1 for the blank option
        if ($('#type-select').children().length == self.EventTypes().length + 1) {
            var typeElement = $('#type-select').first();
            typeElement.chosen({
                width: "100%",
                placeholder_text_single: "Event Type"
            });
        }
    };
    self.MemberListInit = function (elements, data) {
        //i hate this if check, there has to be a more elegant way
        //but i need this to only happen after everything is rendered. +1 for the blank option
        if ($('#member-select').children().length == self.Users().length + 1) {
            var memberElement = $('#member-select').first();
            memberElement.chosen({
                width: "100%",
                placeholder_text_multiple: "Members (Optional)"
            });
            //change event to update the Members observable
            memberElement.on('change', function (ev, params) {
                if (params.selected) {
                    self.NewEvent().Members.push(params.selected);
                }
                if (params.deselected) {
                    self.NewEvent().Members.remove(params.deselected);
                }
            });
        }
    };

    self.CreateEvent = function () {
        self.NewEvent().StartDate(new Date());
        $.ajax({
            url: '/Event/CreateEvent',
            type: 'POST',
            data: {
                data: ko.toJSON(self.NewEvent())
            },
            dataType: 'json',
            success: function (data) {
                if (data.Success) {
                    window.location.href = window.location.origin + '/Event/View/' + data.Payload.EventId;
                }
            }
        });
    };

    self.GetEvents = function (page, pageSize) {
        $.ajax({
            url: '/Event/GetEvents',
            dataType: 'json',
            data: {
                page: page-1,
                pageSize: pageSize
            },
            success: function (data) {
                self.Events($.map(data.Payload.events, function (e) {
                    return new EventModel(e);
                }));
            }
        });
    };

    self.CopyEvent = function (event) {
        // get event for members and options
        $.ajax({
            url: '/Event/GetEvent',
            data: {
                eventId: event.EventId
            },
            dataType: 'json',
            success: function (data) {
                if (data.Success) {
                    // basic operations
                    $('#createEvent').collapse('show');

                    var newEvent = self.NewEvent();

                    newEvent.Name(event.Name());

                    newEvent.TypeId(event.TypeId());
                    var typeElement = $('#type-select').first();
                    typeElement.trigger("chosen:updated");

                    // member and option information from the request
                    newEvent.Options.removeAll();
                    var eventData = data.Payload.event;
                    for (var i = 0; i < eventData.Options.length; i++) {
                        newEvent.Options.push(ko.observable(eventData.Options[i].Name));
                    }

                    var memberElement = $('#member-select').first();
                    var members = (eventData.Members && eventData.IsPrivate) ? $.map(eventData.Members, function (m) {
                        return String(m.UserId);
                    }) : [];
                    memberElement.val(members);
                    newEvent.Members(members);
                    memberElement.trigger("chosen:updated")
                }
            }
        });
    }
};

var EventModel = function(data) {
    var self = this;

    self.EventId = data.EventId;
    self.Name = ko.observable(data.Name);
    self.TypeId = ko.observable(data.TypeId);
    self.IsPrivate = ko.observable(data.IsPrivate);
    self.StartDate = ko.observable(data.StartDate ? new Date(data.StartDate).toLocaleDateString() : null);
    self.EndDate = ko.observable(data.EndDate ? new Date(data.EndDate).toLocaleDateString() : null);
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
    self.AddGameSet = function (gameSet) {
        for (var i = 0; i < gameSet.options.length; i++) {
            self.Options.push(gameSet.options[i]);
        }
    };
    self.RemoveOption = function (option) {
        self.Options.remove(option);
    };

    self.url = "/Event/View/" + self.EventId;
};

var OptionModel = function(data) {
    var self = this;

    self.GameId = data.GameId;
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
            if (data.Success) {
                model.EventTypes($.map(data.Payload.eventTypes, function (e) {
                    return {
                        id: e.TypeId,
                        name: e.Name
                    };
                }));
                model.Users($.map(data.Payload.users, function (u) {
                    return {
                        id: u.UserId,
                        name: u.UserName
                    };
                }));
                model.GameSets($.map(data.Payload.gameSets, function (os) {
                    return {
                        id: os.GameSetsId,
                        name: os.Name,
                        options: $.map(os.Options, function (o) {
                            return {
                                id: o.GameId,
                                name: o.Name
                            };
                        })
                    };
                }));
                model.EventCount(data.Payload.eventCount);
            }
        }
    });

    // get initial events with the default settings
    model.GetEvents(0, 20);
});