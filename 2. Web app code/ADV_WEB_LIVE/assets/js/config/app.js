var App = angular.module('myApp', ['ngCookies']);


App.directive('loadingss', ['$http', function ($http) {
      return {
          restrict: 'A',
          link: function (scope, element, attrs) {
              scope.isLoading = function () {
                  return $http.pendingRequests.length > 0;
              };
              scope.$watch(scope.isLoading, function (value) {
                  if (value) {
                      $("#loadingss").animate({top: '0px'},800);
                  } else {
                      $("#loadingss").animate({ top: '-100px' },800);
                  }
              });
          }
      };
  } ]);

  App.directive('stringToNumber', function () {
      return {
          require: 'ngModel',
          link: function (scope, element, attrs, ngModel) {
              ngModel.$parsers.push(function (value) {
                  return '' + value;
              });
              ngModel.$formatters.push(function (value) {
                  return parseFloat(value, 10);
              });
          }
      }
  })


  App.directive('ngEnter', function () {
      return function (scope, element, attrs) {
          element.bind("keydown keypress", function (event) {
              if (event.which === 13) {
                  scope.$apply(function () {
                      scope.$eval(attrs.ngEnter, { 'event': event });
                  });

                  event.preventDefault();
              }
          });
      };
  });
