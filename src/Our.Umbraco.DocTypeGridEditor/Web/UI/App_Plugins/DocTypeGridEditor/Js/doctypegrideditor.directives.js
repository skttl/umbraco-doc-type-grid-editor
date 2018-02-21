angular.module("umbraco.directives").directive("compile", [

    "$compile",

    function ($compile) {

        var directive = {
            restrict: "A",
            scope: {
                compile: "="
            },
            link: function ($scope, $element, $attrs) {
                $scope.$watch(function () { return $scope.compile }, function (newValue, oldValue) {
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