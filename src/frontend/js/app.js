// Инициализация приложения
(function () {
    // Регистрация маршрутов
    Router.register('dashboard', () => DashboardPage.render());

    // Кастомные страницы для отдельных утилит (переопределяют общую страницу)
    Router.register('utility/text-to-list', () => UtilityTextToListPage.render({ endpoint: 'text-to-list' }));

    // Общая страница для всех остальных утилит
    Router.register('utility/:endpoint', (params) => UtilityPage.render(params));

    // Старт
    Router.init();
})();
