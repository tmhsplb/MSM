

MSMApp.controller('reviewController', ['$scope', '$http', 'MergeManager', 'DTOptionsBuilder', 'DTColumnBuilder',
        function ($scope, $http, MergeManager, DTOptionsBuilder, DTColumnBuilder) {
            $scope.matched = MergeManager.getMatched();
            $scope.unmatched = MergeManager.getUnmatched();
            $scope.tab = 'review';
        }]);