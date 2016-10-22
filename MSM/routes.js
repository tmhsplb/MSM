﻿MSMApp.config(function ($routeProvider, $httpProvider) {
    $routeProvider
      .when('/', { controller: 'homeController', templateUrl: 'partials/home.html' })
      .when('/Merge', { controller: 'mergeController', templateUrl: 'partials/merge.html' })
      .when('/Inspect', { controller: 'inspectController', templateUrl: 'partials/inspect.html' })
      .when('/Research', { controller: 'researchController', templateUrl: 'partials/research.html' })
      .otherwise({ redirectTo: '/' });

    // $httpProvider.defaults.useXDomain = true;
    //  delete $httpProvider.defaults.headers.common['X-Requested-With'];
});



