
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/orderNotificationHub")
    .build();

connection.on("ReceiveOrderNotification", function (notification) {
    toastr.info(notification.message);
    document.title = notification.message

    updateNotification()
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});

// UPDATE NUMBER OF HAVEN'T READ NOTIFICATION



const updateNotification = () => {
    fetch("/api/Notification")
        .then(res => res.json())
        .then(res => {
            renderNotification(res)
        })
}

updateNotification();

function timeAgo(fromDate, toDate) {
    const seconds = Math.floor((toDate - fromDate) / 1000);

    let interval = Math.floor(seconds / 31536000);
    if (interval >= 1) {
        return `${interval} năm trước`;
    }

    interval = Math.floor(seconds / 2592000);
    if (interval >= 1) {
        return `${interval} tháng trước`;
    }

    interval = Math.floor(seconds / 86400);
    if (interval >= 1) {
        return `${interval} ngày trước`;
    }

    interval = Math.floor(seconds / 3600);
    if (interval >= 1) {
        return `${interval} giờ trước`;
    }

    interval = Math.floor(seconds / 60);
    if (interval >= 1) {
        return `${interval} phút trước`;
    }

    return `${seconds} giây trước`;
}

const renderNotification = (data) => {
    const notificationPopupTag = document.getElementById('notification-wrapper')

    const html = data.map(item => {
        const today = new Date(); 
        const pastDate = new Date(item.createdAt); 
        return `
                    <li>
                            <hr class="dropdown-divider">
                        </li>

                        <li class="notification-item">
                            <i class="bi bi-exclamation-circle text-warning"></i>
                            <div>
                                <h4>${item.title}</h4>
                                    <p>${item.message}</p>
                                <p>${timeAgo(pastDate, today)}</p>
                            </div>
                        </li>
            `
    }).join('');

    const count = data.filter(item => !item.haveRead).length;
    const numberOrNotifications = document.getElementById('number-notification')
    numberOrNotifications.innerText = count;

    notificationPopupTag.innerHTML = `
            <li class="dropdown-header">
                    You have ${count} new notifications
                <a href="#"><span class="badge rounded-pill bg-primary p-2 ms-2">View all</span></a>
            </li>
        `;

    notificationPopupTag.innerHTML += html;
}