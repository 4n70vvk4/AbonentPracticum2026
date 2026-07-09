// Страница утилиты «Текст в список C# / JS» — кастомный UI с выбором языка
const UtilityTextToListPage = {
    async render(params) {
        const content = document.getElementById('app-content');
        const endpoint = params.endpoint;

        content.innerHTML = '<div class="loading">Загрузка утилиты</div>';

        try {
            const [utility, history] = await Promise.all([
                API.get(`/utilities/${endpoint}`),
                API.get(`/utilities/${endpoint}/history?limit=10`)
            ]);

            const stars = '★'.repeat(utility.difficulty) + '☆'.repeat(3 - utility.difficulty);

            const languages = [
                { id: 'csharp',    label: 'C#' },
                { id: 'javascript', label: 'JavaScript' },
                { id: 'python',    label: 'Python' },
                { id: 'sql',       label: 'SQL' },
            ];

            let html = `
                <a href="#dashboard" class="back-link">← Назад к списку</a>
                <div class="utility-detail">
                    <h1>${utility.name}</h1>
                    <div class="meta">
                        <span class="badge badge-category">${utility.category}</span>
                        <span class="difficulty-stars">${stars}</span>
                        <span class="badge badge-ready">Реализована</span>
                    </div>
                    <p>${utility.description}</p>
                </div>

                <div class="utility-detail">
                    <div class="input-group">
                        <label>Язык:</label>
                        <div class="lang-buttons" id="lang-buttons">
                            ${languages.map(l => `
                                <button class="lang-btn" data-lang="${l.id}">${l.label}</button>
                            `).join('')}
                        </div>
                    </div>
                    <div class="input-group">
                        <label for="util-input">Входные данные:</label>
                        <textarea id="util-input" placeholder="Введите элементы списка, по одному на строку..."></textarea>
                    </div>
                    <button class="btn btn-primary" id="btn-execute">▶ Выполнить</button>
                    <div id="exec-result"></div>
                </div>`;

            // История
            if (history && history.length > 0) {
                html += `<div class="history-section">
                    <h2>📋 История выполнений</h2>`;
                history.forEach(h => {
                    html += `
                    <div class="history-item">
                        <div class="history-time">${new Date(h.executedAt).toLocaleString('ru-RU')}</div>
                        <div class="history-io">
                            <div><strong>Вход:</strong><code>${this.escape(h.input)}</code></div>
                            <div><strong>Выход:</strong><code>${this.escape(h.output)}</code></div>
                        </div>
                    </div>`;
                });
                html += `</div>`;
            }

            content.innerHTML = html;

            // --- Логика кнопок языка ---
            const langButtons = document.querySelectorAll('.lang-btn');
            let selectedLang = null;

            langButtons.forEach(btn => {
                btn.addEventListener('click', () => {
                    langButtons.forEach(b => b.classList.remove('active'));
                    btn.classList.add('active');
                    selectedLang = btn.dataset.lang;
                });
            });

            // --- Обработчик кнопки «Выполнить» ---
            document.getElementById('btn-execute').addEventListener('click', async () => {
                const input = document.getElementById('util-input').value;
                const resultDiv = document.getElementById('exec-result');

                if (!selectedLang) {
                    resultDiv.innerHTML = '<div class="error-message">Выберите язык.</div>';
                    return;
                }

                if (!input.trim()) {
                    resultDiv.innerHTML = '<div class="error-message">Введите элементы списка.</div>';
                    return;
                }

                const fullInput = `${selectedLang}\n${input}`;

                resultDiv.innerHTML = '<div class="loading">Выполнение</div>';

                try {
                    const result = await API.post(`/utilities/${endpoint}/execute`, { input: fullInput });
                    if (result.success) {
                        resultDiv.innerHTML = `
                            <div class="output-area">
                                <label>Результат:</label>
                                <pre>${this.escape(result.output)}</pre>
                            </div>`;
                    } else {
                        resultDiv.innerHTML = `<div class="error-message">${this.escape(result.error)}</div>`;
                    }
                } catch (err) {
                    resultDiv.innerHTML = `<div class="error-message">Ошибка: ${err.message}</div>`;
                }
            });

            // Ctrl+Enter для выполнения
            document.getElementById('util-input').addEventListener('keydown', (e) => {
                if (e.ctrlKey && e.key === 'Enter') {
                    document.getElementById('btn-execute').click();
                }
            });

        } catch (err) {
            content.innerHTML = `
                <a href="#dashboard" class="back-link">← Назад к списку</a>
                <div class="error-message">Ошибка загрузки: ${err.message}</div>`;
        }
    },

    escape(str) {
        if (!str) return '';
        str = String(str);
        return str
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;');
    }
};
