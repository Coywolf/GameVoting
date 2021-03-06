﻿var ViewModel = function (data) {
    var self = this;

    self.EventId = data.EventId;
    self.Name = ko.observable(data.Name);
    self.TypeId = data.TypeId;
    self.IsPrivate = ko.observable(data.IsPrivate);
    self.StartDate = data.StartDate;
    self.EndDate = ko.observable(data.EndDate);
    self.Creator = ko.observable(data.Creator);
    self.EventType = data.EventType;
    self.TypeDescription = data.TypeDescription;
    self.HasVoted = ko.observable(data.HasVoted);
    self.IsCreator = data.IsCreator;

    self.Title = ko.pureComputed(function() {
        return self.Name();
    });
    self.Subtitle = ko.pureComputed(function () {
        var str = (new Date(self.StartDate)).toLocaleDateString();
        if(self.EndDate()) {
            str = str + " - " + (new Date(self.EndDate())).toLocaleDateString();
        }
        return str;
    });
    self.Description = ko.pureComputed(function () {
        return self.EventType + " - " + self.TypeDescription;
    });

    self.CanVote = ko.pureComputed(function() {
        return !self.HasVoted() && !self.EndDate();
    });

    self.MinScore = data.MinScore == undefined ? data.Options.length : data.MinScore;
    self.MaxScore = data.MaxScore == undefined ? data.Options.length : data.MaxScore;

    self.Results = ko.observable().extend({ resultsChart: "results-chart" });
    var resultSubscription = self.Results.subscribe(function () {
        $.connection.hub.qs = { "eventId": self.EventId };
        $.connection.eventHub.client.UpdateResults = function(results) {
            self.Results(results);
        };
        $.connection.eventHub.client.Refresh = function() {
            location.reload();
        };
        $.connection.hub.start();
        
        //remove the subscription, so this will only fire once.
        resultSubscription.dispose();
    });
    self.VoteCount = ko.pureComputed(function() {
        if(self.Results()) {
            return "(" + self.Results().NumberOfVotes + " votes)";
        }
        else {
            return "";
        }
    });

    self.Options = $.map(data.Options, function (o) {
        return new OptionModel(o, self);
    });
    self.Members = ko.observableArray($.map(data.Members, function (m) {
        return new MemberModel(m);
    }));
    self.ShowMembers = ko.pureComputed(function() {
        return self.IsPrivate() || self.EndDate();
    });
    self.MakeOptionUnique = function (option, replaceWith) {
        var score = option.Score();

        for (var i = 0; i < self.Options.length; i++) {
            var curOption = self.Options[i];
            if (curOption != option && curOption.Score() == score) {
                curOption.Score(replaceWith);
            }
        }
    };

    self.Submit = function (deferred) {
        $.ajax({
            url: '/Event/SubmitVote',
            type: 'POST',
            dataType: 'json',
            data: {
                eventId: self.EventId,
                voteData: deferred ? "[]" : ko.toJSON(self.Options),
                deferred: deferred
            },
            success: function (data) {
                if (data.Success) {
                    self.HasVoted(true);
                    $.ajax({
                        url: '/Event/GetEventResults',
                        dataType: 'json',
                        data: {
                            eventId: self.EventId
                        },
                        success: function (results) {
                            if (results.Success) {
                                self.Results(results.Payload);
                            }
                        }
                    });
                }
                else {
                    console.error(data.Message);
                }
            }
        });
    };

    self.CloseEvent = function() {
        $.ajax({
            url: '/Event/CloseEvent',
            type: 'POST',
            dataType: 'json',
            data: {
                eventId: self.EventId
            },
            success: function (data) {
                if (data.Success) {
                    location.reload();
                }
                else {
                    console.error(data.Message);
                }
            }
        });
    };

    self.AddingMembers = ko.observable(false);
    self.AddMemberButtonStyle = ko.pureComputed(function() {
        return self.AddingMembers() ? "btn-primary" : "btn-default";
    });
    self.Users = ko.observableArray();
    self.NewMembers = ko.observableArray();
    self.AddMember = function() {
        if (self.AddingMembers()) {
            $.ajax({
                url: '/Event/AddMembers',
                type: 'POST',
                data: {
                    eventId: self.EventId,
                    memberIds: self.NewMembers()
                },
                dataType: 'json',
                success: function (result) {
                    if (result.Success) {
                        self.Members($.map(result.Payload, function (m) {
                            return new MemberModel(m);
                        }));
                        self.Users.removeAll();
                        self.AddingMembers(false);
                    }
                }
            });
        }
        else {
            $.ajax({
                url: '/Event/GetUsersForAdding',
                data: {
                    eventId: self.EventId
                },
                dataType: 'json',
                success: function (result) {
                    if(result.Success) {
                        self.Users($.map(result.Payload, function(u) {
                            return {
                                id: u.UserId,
                                name: u.UserName
                            };
                        }));
                        self.AddingMembers(true);
                    }
                }
            });
        }
    };
    self.AddMembersInit = function() {
        //i hate this if check, there has to be a more elegant way
        //but i need this to only happen after everything is rendered. +1 for the blank option
        if ($('#member-select').children().length == self.Users().length + 1) {
            var memberElement = $('#member-select').first();
            memberElement.chosen({
                width: "100%",
                placeholder_text_multiple: "New Members"
            });
            //change event to update the NewMembers observable
            memberElement.on('change', function (ev, params) {
                if (params.selected) {
                    self.NewMembers.push(params.selected);
                }
                if (params.deselected) {
                    self.NewMembers.remove(params.deselected);
                }
            });
        }
    };
};

var OptionModel = function (data, parent) {
    var self = this;

    self.OptionId = data.OptionId;
    self.Name = data.Name;
    self.Score = ko.observable(data.Score);

    self.Score.subscribe(function (newValue) {
        if (parent.EventType == "Favorite") {
            //'1' must be unique
            if (newValue == 1) {
                parent.MakeOptionUnique(self, 0);
            }
        }
        else if (parent.EventType == "Ok") {
            //no restriction
        }
        else if (parent.EventType == "Ok-Rank") {
            //no restriction
        }
        else if (parent.EventType == "Rank") {
            //All values must be unique
            parent.MakeOptionUnique(self);
        }
    });
};

var MemberModel = function (data) {
    var self = this;

    self.UserId = data.UserId;
    self.UserName = ko.observable(data.UserName);
    self.HasDeferred = ko.observable(data.HasDeferred);
};

var debug;
var debug2;
$(document).ready(function () {
    var splitUrl = window.location.href.split('/');
    var eventArg = splitUrl[splitUrl.length - 1];  //get the last element, which should be the id
    
    //make sure this is a valid number
    $.ajax({
        url: '/Event/GetEvent',
        data: {
            eventId: eventArg
        },
        dataType: 'json',
        success: function (data) {
            if (data.Success) {
                var model = new ViewModel(data.Payload.event);
                debug2 = model;
                if (data.Payload.results) {
                    model.Results(data.Payload.results);
                    debug = data.Payload.results;
                }
                ko.applyBindings(model);
            }
            else {
                window.location = "/";
            }
        }
    });
});

vote_selectValue = function (element, value) {
    //set all of the score buttons back to default
    $(element).find('button.score-button').removeClass('btn-info').addClass('btn-default');
    //set the button matching the passed value to warning
    $(element).find('button.score-button[data-value="' + value + '"]').removeClass('btn-default').addClass('btn-info');
};

ko.bindingHandlers.vote = {
    init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
        var options = valueAccessor();

        if (options.min <= options.max) {
            //create the scoring buttons
            var btnGroup = $('<div/>', {
                'class': 'btn-group',
                role: 'group'
            });

            for (var i = options.min; i <= options.max; i++) {
                var btn = $('<button/>', {
                    'class': 'btn btn-default btn-lg score-button',
                    'data-value': i,
                    text: i
                });
                if (!options.enable()) {
                    btn.prop("disabled", true);
                }
                btn.click(function () {
                    options.value($(this).attr('data-value'));
                });
                btnGroup.append(btn);
            }
            $(element).append(btnGroup);

            //set the initially selected button
            var value = ko.unwrap(options.value);
            vote_selectValue(element, value);

            //subscribe to the score to update the selected button
            options.value.subscribe(function (newValue) {
                vote_selectValue(element, newValue);
            });
            options.enable.subscribe(function (newValue) {
                if (newValue) {
                    $(element).find('.score-button').prop("disabled", false);
                }
                else {
                    $(element).find('.score-button').prop("disabled", true);
                }
            });
        }
    }  
};

ko.extenders.resultsChart = function(target, selector) {
    target.subscribe(function(nv) {
        //initialization might need a delay. this is relying on the 'with' binding in the html happening before this subscription
        if (target.chart === undefined) {
            //initialize chart
            var chartOptions = {
                chart: {
                    renderTo: selector,
                    type: 'bar'
                },
                title: {
                    text: null
                },
                xAxis: {
                    categories: $.map(nv.Options, function(option) { return option.Name; })
                },
                yAxis: [{
                    title: {
                        text: 'Scores'
                    },
                    allowDecimals: false
                }, {
                    title: {
                        text: null
                    },
                    linkedTo: 0,
                    opposite: true
                }],
                legend: {
                    enabled: false
                },
                tooltip: {
                    shared: true
                },
                plotOptions: {
                    bar: {
                        grouping: false
                    }
                },
                series: [{
                    name: 'Score',
                    data: $.map(nv.Options, function(option) { return option.Score; }),
                    legendIndex: 0
                }]
            };

            if (nv.ShowWeights) {
                chartOptions.legend.enabled = true;
                chartOptions.series[0].pointPadding = 0.2;
                chartOptions.series.unshift({
                    name: 'Weight',
                    data: $.map(nv.Options, function(option) { return option.Weight; }),
                    legendIndex: 1,
                    pointPadding: 0.0
                });
            }

            $("#"+selector).height(nv.Options.length * 40 + 145);
            target.chart = new Highcharts.Chart(chartOptions);
        } else {
            //update chart
            target.chart.xAxis[0].setCategories($.map(nv.Options, function(option) { return option.Name; }), false);

            //not expecting this value to change with an update. This is based on chart type
            if (nv.ShowWeights) {
                target.chart.series[0].setData($.map(nv.Options, function(option) { return option.Weight; }), false);
                target.chart.series[1].setData($.map(nv.Options, function(option) { return option.Score; }), false);
            } else {
                target.chart.series[0].setData($.map(nv.Options, function(option) { return option.Score; }), false);
            }
            target.chart.redraw();
        }
    });

    return target;
};

ko.bindingHandlers.slideVisible = {
    update: function (element, valueAccessor, allBindings) {
        // First get the latest data that we're bound to
        var value = valueAccessor();

        // Next, whether or not the supplied model property is observable, get its current value
        var valueUnwrapped = ko.unwrap(value);

        // Grab some more data from another binding property
        var duration = allBindings.get('slideDuration') || 400; // 400ms is default duration unless otherwise specified

        // Now manipulate the DOM element
        if (valueUnwrapped == true)
            $(element).slideDown(duration); // Make the element visible
        else
            $(element).slideUp(duration);   // Make the element invisible
    }
};