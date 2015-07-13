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
            var numPages = Math.ceil(ko.unwrap(numItems) / self.pageSize());

            return Array.apply(null, { length: numPages }).map(function (u, i) {
                var page = {
                    page: i+1
                };
                page.btnClass = ko.pureComputed(function () {
                    return this.page == self.currentPage() ? 'btn-primary' : 'btn-default';
                }, page);

                return page;
            });
        });
        self.visiblePages = ko.pureComputed(function () {
            var numPages = self.pages().length;

            var start = Math.max(0, Math.min(self.currentPage() - 4, numPages - 7));
            var end = Math.min(start + 7, numPages);

            return self.pages().slice(start, end);
        });

        var pageChanged = function () {
            if (params.pageChanged && $.isFunction(params.pageChanged)) {
                params.pageChanged.call(self, self.currentPage(), self.pageSize());
            }
        }
        self.currentPage.subscribe(pageChanged);
        self.pageSize.subscribe(pageChanged);

        self.setPage = function (page) {
            self.currentPage(page.page);
        };

        self.first = function () {
            self.currentPage(1);
        };

        self.previous = function () {
            if (self.currentPage() > 1) {
                self.currentPage(self.currentPage() - 1);
            }
        };

        self.next = function () {
            if (self.currentPage() < self.pages().length) {
                self.currentPage(self.currentPage() + 1);
            }
        };

        self.last = function () {
            self.currentPage(self.pages().length);
        };
    },
    template: { element: 'page-controls-template' }
})