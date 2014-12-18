var ViewModel = function (data) {
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
    self.CanClose = data.CanClose;

    self.CanVote = ko.pureComputed(function() {
        return self.HasVoted() || self.EndDate();
    });

    self.MinScore = data.MinScore == undefined ? data.Options.length : data.MinScore;
    self.MaxScore = data.MaxScore == undefined ? data.Options.length : data.MaxScore;

    self.Results = ko.observable().extend({ resultsChart: "results-chart" });

    self.Options = $.map(data.Options, function (o) {
        return new OptionModel(o, self);
    });
    self.Members = $.map(data.Members, function (m) {
        return new MemberModel(m);
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

    self.Submit = function () {
        $.ajax({
            url: '/Event/SubmitVote',
            type: 'POST',
            dataType: 'json',
            data: {
                eventId: self.EventId,
                voteData: ko.toJSON(self.Options)
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
                    self.EndDate(data.Payload);
                }
                else {
                    console.error(data.Message);
                }
            }
        });
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
            eventArg: eventArg
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
                if (options.readonly()) {
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
            options.readonly.subscribe(function (newValue) {
                if (newValue) {
                    $(element).find('.score-button').prop("disabled", true);
                }
                else {
                    
                    $(element).find('.score-button').prop("disabled", false);
                }
            });
        }
    }  
};

ko.extenders.resultsChart = function (target, selector) {
    target.subscribe(function (nv) {
        //initialization might need a delay. this is relying on the 'with' binding in the html happening before this subscription
        if(target.chart === undefined) {
            //initialize chart
            var chartOptions = {
                chart: {
                    renderTo: selector,
                    type: 'column'
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
                        }
                    }],
                tooltip: {
                    shared: true
                },
                plotOptions: {
                    column: {
                        grouping: false
                    }
                },
                series: [{
                        name: 'Score',
                        data: $.map(nv.Options, function(option) { return option.Score; }),
                        legendIndex: 0
                    }]
            };
            
            if(nv.ShowWeights) {
                chartOptions.yAxis.push({
                    title: {
                        text: 'Weights'
                    },
                    opposite: true
                });
                chartOptions.series[0].pointPadding = 0.2;
                chartOptions.series.unshift({
                    name: 'Weight',
                    data: $.map(nv.Options, function(option) { return option.Weight; }),
                    yAxis: 1,
                    pointPadding: 0.0,
                    legendIndex: 1
                });
            }
            
            target.chart = new Highcharts.Chart(chartOptions);
        }
        else {
            //update chart
            target.chart.xAxis[0].setCategories($.map(nv.Options, function (option) { return option.Name; }), false);
            target.chart.series[0].setData($.map(nv.Options, function(option) { return option.Weight; }), false);
            target.chart.series[1].setData($.map(nv.Options, function (option) { return option.Score; }), false);
            target.chart.redraw();
        }
    });

    return target;
}