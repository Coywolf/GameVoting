ko.components.register('page-controls', {
    viewModel: function (params) {
        var self = this;

        var numItems = params.numItems;

        self.currentPage = ko.observable(1);
        self.pageSize = ko.observable(20);

        self.currentPageDisplay = ko.pureComputed(function () {
            var numPages = self.pages().length;
            var startItem = (self.currentPage() - 1) * self.pageSize() + 1;
            var endItem = self.currentPage() * self.pageSize();

            return "of " + numPages + " (" + startItem + "-" + endItem + " of " + ko.unwrap(numItems) + ")";
        });

        self.pages = ko.pureComputed(function () {
            var numPages = ko.unwrap(numItems) / self.pageSize();
            return Array.apply(null, { length: Math.ceil(numPages) }).map(function (u, i) {
                var page = {
                    page: i+1
                };
                page.btnClass = ko.pureComputed(function () {
                    return this.page == self.currentPage() ? 'btn-primary' : 'btn-default';
                }, page);

                return page;
            });
        });

        self.currentPage.subscribe(function (newValue) {
            if (params.pageChanged && $.isFunction(params.pageChanged)) {
                params.pageChanged.call(self, newValue, self.pageSize());
            }
        });

        self.setPage = function (page) {
            self.currentPage(page.page);
        };

        self.first = function () {
            self.currentPage(0);
        };

        self.previous = function () {
            if (self.currentPage() > 0) {
                self.currentPage(self.currentPage() - 1);
            }
        };

        self.next = function () {
            if (self.currentPage() < self.pages().length - 1) {
                self.currentPage(self.currentPage() + 1);
            }
        };

        self.last = function () {
            self.currentPage(self.pages().length-1);
        };
    },
    template: { element: 'page-controls-template' }
})