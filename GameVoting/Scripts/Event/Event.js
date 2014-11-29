var ViewModel = function (data) {
    var self = this;

    self.EventId = data.EventId;
    self.Name = ko.observable(data.Name);
    self.TypeId = ko.observable(data.TypeId);
    self.IsPrivate = ko.observable(data.IsPrivate);
    self.StartDate = data.StartDate;
    self.EndDate = ko.observable(data.EndDate);
    self.Creator = ko.observable(data.Creator);
    self.EventType = ko.observable(data.EventType);

    self.MinScore = data.MinScore == undefined ? data.Options.length : data.MinScore;
    self.MaxScore = data.MaxScore == undefined ? data.Options.length : data.MaxScore;

    self.Options = $.map(data.Options, function (o) {
        return new OptionModel(o, self);
    });
};

self.OptionModel = function (data, parent) {
    var self = this;

    self.OptionId = data.OptionId;
    self.Name = data.Name;
    self.Score = ko.observable(parent.MinScore);
};

$(document).ready(function () {
    var splitUrl = window.location.href.split('/');
    var eventId = splitUrl[splitUrl.length - 1];  //get the last element, which should be the id

    //make sure this is a valid number
    if (!isNaN(eventId)) {
        $.ajax({
            url: '/Event/GetEvent',
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
        }
    }  
};