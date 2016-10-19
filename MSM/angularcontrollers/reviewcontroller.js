

MSMApp.controller('reviewController', ['$scope', '$http', 'MergeManager', 'DTOptionsBuilder', 'DTColumnBuilder',
        function ($scope, $http, MergeManager, DTOptionsBuilder, DTColumnBuilder) {
            $scope.matched = MergeManager.getMatchedChecks();
            $scope.unmatched = MergeManager.getUnmatchedChecks();
            $scope.tab = 'review';
        }]);