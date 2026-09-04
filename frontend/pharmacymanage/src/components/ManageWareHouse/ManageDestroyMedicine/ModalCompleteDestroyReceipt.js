import { useState, useEffect } from 'react';
import Button from 'react-bootstrap/Button';
import Modal from 'react-bootstrap/Modal';
import { toast } from 'react-toastify';

import { GetDestroyReceiptById, CompleteDestroyReceipt } from '../../../services/apiService';

const ModalCompleteDestroyReceipt = (props) => {
    const { show, setShow, dataComplete, fetchDestroyReceipt } = props;

    const [detail, setDetail] = useState(null);
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        if (show && dataComplete && dataComplete.destroyReceiptID) {
            fetchDetail(dataComplete.destroyReceiptID);
        }
    }, [show, dataComplete]);

    const fetchDetail = async (id) => {
        setLoading(true);
        let res = await GetDestroyReceiptById(id);
        if (res && res.ec === 0 && res.dt) {
            setDetail(res.dt);
        } else {
            setDetail(null);
        }
        setLoading(false);
    };

    const handleClose = () => {
        setShow(false);
        setDetail(null);
    };

    const handleSubmitComplete = async () => {
        let res = await CompleteDestroyReceipt(dataComplete.destroyReceiptID);
        if (!res || res.ec !== 0) {
            toast.error(res?.em || 'Hoàn thành phiếu tiêu hủy thất bại');
            return;
        }

        toast.success(res.em || 'Hoàn thành phiếu tiêu hủy thành công');
        handleClose();
        await fetchDestroyReceipt(1);
    };

    return (
        <Modal show={show} onHide={handleClose} centered className='modal-complete-destroy-receipt'>
            <Modal.Header closeButton>
                <Modal.Title>Hoàn thành phiếu tiêu hủy #{dataComplete.destroyReceiptID}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                {loading && <div className="text-center text-muted">Đang tải...</div>}
                {!loading && detail && (
                    <>
                        <p>Bạn có chắc chắn muốn hoàn thành phiếu tiêu hủy #{dataComplete.destroyReceiptID} không?</p>
                        <div className="row g-3 mt-2">
                            <div className="col-md-6">
                                <label className="fw-bold">Phiếu kiểm kê</label>
                                <div>#{detail.stockTakeID}</div>
                            </div>
                            <div className="col-md-6">
                                <label className="fw-bold">Kho</label>
                                <div>{detail.warehouseName}</div>
                            </div>
                            <div className="col-md-6">
                                <label className="fw-bold">Số mặt hàng</label>
                                <div>{detail.items?.length || 0} dòng</div>
                            </div>
                        </div>
                    </>
                )}
            </Modal.Body>
            <Modal.Footer>
                <Button variant="secondary" onClick={handleClose}>
                    Đóng
                </Button>
                <Button variant="success" onClick={() => handleSubmitComplete()} disabled={loading}>
                    Hoàn thành
                </Button>
            </Modal.Footer>
        </Modal>
    );
};

export default ModalCompleteDestroyReceipt;
