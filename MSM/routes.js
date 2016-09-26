MSMApp.config(function ($routeProvider, $httpProvider) {
    $routeProvider
      .when('/', { controller: 'homeController', templateUrl: 'partials/home.html' })
      .when('/Merge', { controller: 'mergeController', templateUrl: 'partials/merge.html' })
      .when('/Inspect', { controller: 'inspectController', templateUrl: 'partials/inspect.html' })
      .when('/Review', { controller: 'reviewController', templateUrl: 'partials/review.html' })
     // .when('/Quickbooks', { controller: 'quickbooksController', templateUrl: 'partials/quickbooks.html' })
    //  .when('/Examples', { controller: 'examplesController', templateUrl: 'partials/examples.html' })
    //  .when('/Extras', { controller: 'extrasController', templateUrl: 'partials/extras.html' })
    //  .when('/Sunyear', { controller: 'sunyearController', templateUrl: 'partials/sunyear.html' })
      .otherwise({ redirectTo: '/' });

    // $httpProvider.defaults.useXDomain = true;
    //  delete $httpProvider.defaults.headers.common['X-Requested-With'];
});



