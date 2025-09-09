fetch('/api/dashboard/weekly').then(r=>r.json()).then(data=>{
  const ctx = document.getElementById('weeklyChart');
  if (!ctx) return;
  new Chart(ctx, {
    type: 'bar',
    data: { labels: data.labels, datasets: [{ label: '支出', data: data.amounts }] }
  });
}).catch(console.error);