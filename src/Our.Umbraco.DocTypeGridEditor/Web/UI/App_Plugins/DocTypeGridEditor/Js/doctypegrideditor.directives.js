angular.module("umbraco.directives").directive("dtgeBindHtmlCompile", [

    "$compile",

    function ($compile) {

        var directive = {
            restrict: "A",
            scope: {
                dtgeBindHtmlCompile: "="
            },
            link: function ($scope, $element, $attrs) {
                $scope.$watch(function () { return $scope.dtgeBindHtmlCompile }, function (newValue, oldValue) {
                    $element.html(newValue);

                    // Only compile child nodes to avoid starting an infinite loop
                    var childNodes = $element.contents();
                    $compile(childNodes)($scope);
                });
            }
        };

        return directive;
    }
]);