fetch('/api/dashboard/weekly')
  .then(r => r.json())
  .then(data => {
    // チャート表示
    const ctx = document.getElementById('weeklyChart');
    if (ctx) {
      new Chart(ctx, {
        type: 'bar',
        data: {
          labels: data.labels,
          datasets: [{
            label: '支出',
            data: data.amounts,
            backgroundColor: 'rgba(75, 192, 192, 0.6)',
            borderColor: 'rgba(75, 192, 192, 1)',
            borderWidth: 1
          }]
        },
        options: {
          responsive: true,
          plugins: {
            legend: {
              display: true,
              position: 'top'
            }
          },
          scales: {
            y: {
              beginAtZero: true,
              title: {
                display: true,
                text: '支出額 (¥)'
              }
            }
          }
        }
      });
    }

    // 表形式データ表示
    const tbody = document.getElementById('dailySummaryBody');
    if (tbody && data.dailySummaries) {
      let totalAmount = 0;
      tbody.innerHTML = data.dailySummaries.map(day => {
        totalAmount += day.amount;
        return `<tr>
          <td>${day.date}</td>
          <td>${day.dayOfWeek}</td>
          <td class="text-end">¥${day.amount.toLocaleString('ja-JP', { minimumFractionDigits: 0, maximumFractionDigits: 0 })}</td>
          <td class="text-center">${day.transactionCount}</td>
        </tr>`;
      }).join('');

      // 合計を表示
      const totalElement = document.getElementById('totalAmount');
      if (totalElement) {
        totalElement.textContent = '¥' + totalAmount.toLocaleString('ja-JP', { minimumFractionDigits: 0, maximumFractionDigits: 0 });
      }
    }
  })
  .catch(console.error);