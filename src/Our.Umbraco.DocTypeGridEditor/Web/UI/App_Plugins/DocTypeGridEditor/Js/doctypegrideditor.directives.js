angular
    .module('umbraco.directives')
    .directive('compile', compile);

compile.$inject = ['$compile'];

function compile($compile) {
    var directive = {
        restrict: 'A',
        scope: {
            compile: '='
        },
        link: link
    }

    return directive;

    function link($scope, $element, $attrs) {
        $scope.$watch(function () { return $scope.compile }, function (newValue, oldValue) {
            $element.html(newValue);

            //Only compile child nodes to avoid starting an infinite loop
            var childNodes = $element.contents();
            $compile(childNodes)($scope);
        });
    }
}