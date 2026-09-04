import React, { useState } from 'react';
import Modal from 'react-bootstrap/Modal';
import Button from 'react-bootstrap/Button';
import { toast } from 'react-toastify';

const MODE_CONFIG = {
    approve: {
        title: 'Duyệt phiếu',
        confirmText: 'Duyệt',
        variant: 'success',
        message: (name, code) => `Bạn có chắc chắn muốn duyệt ${name} #${code} không?`
    },
    complete: {
        title: 'Hoàn thành phiếu',
        confirmText: 'Hoàn thành',
        variant: 'success',
        message: (name, code) => `Bạn có chắc chắn muốn hoàn thành ${name} #${code} không?`
    },
    reject: {
        title: 'Từ chối phiếu',
        confirmText: 'Từ chối',
        variant: 'danger',
        message: (name, code) => `Bạn có chắc chắn muốn từ chối ${name} #${code} không?`
    },
    delete: {
        title: 'Xóa phiếu',
        confirmText: 'Xóa',
        variant: 'danger',
        message: (name, code) => `Bạn có chắc chắn muốn xóa ${name} #${code} không?`
    }
};

const StatusActionModal = (props) => {
    const { show, setShow, mode, entityName, record, onConfirm } = props;
    const [reason, setReason] = useState('');

    const config = MODE_CONFIG[mode] || MODE_CONFIG.approve;

    const handleClose = () => {
        setReason('');
        setShow(false);
    };

    const handleSubmit = async () => {
        try {
            if (mode === 'reject') {
                await onConfirm(record, reason);
            } else {
                await onConfirm(record);
            }
            toast.success('Thao tác thành công');
            handleClose();
        } catch (e) {
            toast.error(e?.getMessage?.() || e?.message || 'Thao tác thất bại');
        }
    };

    const code = record?.idLabel
        ? record[record.idLabel]
        : (record?.stockTakeID ?? record?.stockAdjustmentID ?? record?.destroyReceiptID ?? record?.goodsReceiptID ?? record?.id ?? '');

    return (
        <Modal show={show} onHide={handleClose} centered className="modal-status-action">
            <Modal.Header closeButton>
                <Modal.Title>{config.title} {entityName}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <p>{config.message(entityName, code)}</p>
                {mode === 'reject' && (
                    <div className="mt-3">
                        <label className="form-label">Lý do từ chối ({reason ? reason.length : 0}/500)</label>
                        <textarea
                            className="form-control"
                            rows={3}
                            maxLength={500}
                            value={reason}
                            onChange={(e) => setReason(e.target.value)}
                            placeholder="Nhập lý do (không bắt buộc)..."
                        />
                    </div>
                )}
            </Modal.Body>
            <Modal.Footer>
                <Button variant="secondary" onClick={handleClose}>
                    Đóng
                </Button>
                <Button variant={config.variant} onClick={() => handleSubmit()}>
                    {config.confirmText}
                </Button>
            </Modal.Footer>
        </Modal>
    );
};

export default StatusActionModal;
