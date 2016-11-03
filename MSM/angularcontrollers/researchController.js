

MSMApp.controller('researchController', ['$rootScope', '$scope', '$http', '$route', 'ResearchManager', 'DTOptionsBuilder', 'DTColumnBuilder',
        function ($rootScope, $scope, $http, $route, ResearchManager, DTOptionsBuilder, DTColumnBuilder) {
            $rootScope.pageTitle = "Main Street Ministries - Research";
            $scope.tab = 'research';
            $scope.integerval = /^\d*$/;
            $scope.resolvedCheck = "";
            $scope.ResolvedStatus = ResearchManager.getResolvedStatus();

            $scope.ResolveCheck = function () {
              //  console.log("Resolved check: " + $scope.resolvedCheck);
                
                ResearchManager.resolve($scope.resolvedCheck).then(function (r) {
                 
                    //  $scope.ResolvedStatus = [r].join(", ");

                    ResearchManager.setResolvedStatus(r);
                    $route.reload();
                    
                })   
            }
        }]);