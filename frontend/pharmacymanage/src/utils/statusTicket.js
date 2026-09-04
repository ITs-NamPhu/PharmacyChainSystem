export const STATUS_TICKET = {
    PENDING: { key: "PENDING", label: "Chờ duyệt", badge: "bg-warning text-dark" },
    APPROVED: { key: "APPROVED", label: "Đã duyệt", badge: "bg-success" },
    COMPLETE: { key: "COMPLETE", label: "Hoàn thành", badge: "bg-primary" },
    REJECTED: { key: "REJECTED", label: "Bị từ chối", badge: "bg-danger" }
};

export const getStatusInfo = (status) => {
    return STATUS_TICKET[status] || { key: status, label: status || "", badge: "bg-secondary" };
};

export const getStatusBadge = (status) => {
    const info = getStatusInfo(status);
    return <span className={`badge ${info.badge}`}>{info.label}</span>;
};

export const statusLabel = (status) => getStatusInfo(status).label;
