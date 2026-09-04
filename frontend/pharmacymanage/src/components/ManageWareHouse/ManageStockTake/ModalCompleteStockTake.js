import { useState, useEffect } from 'react';
import Button from 'react-bootstrap/Button';
import Modal from 'react-bootstrap/Modal';
import { toast } from 'react-toastify';

import { GetStockTakeById, CompleteStockTake } from '../../../services/apiService';

const ModalCompleteStockTake = (props) => {
    const { show, setShow, dataComplete, fetchStockTake } = props;

    const [detail, setDetail] = useState(null);
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        if (show && dataComplete && dataComplete.stockTakeID) {
            fetchDetail(dataComplete.stockTakeID);
        }
    }, [show, dataComplete]);

    const fetchDetail = async (id) => {
        setLoading(true);
        let res = await GetStockTakeById(id);
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

    const adjustCount = detail?.items?.filter(i => i.adjustQuantity !== 0).length || 0;
    const destroyCount = detail?.items?.filter(i => i.destroyQuantity > 0).length || 0;

    const handleSubmitComplete = async () => {
        let res = await CompleteStockTake(dataComplete.stockTakeID, []);
        if (!res || res.ec !== 0) {
            toast.error(res?.em || 'Hoàn thành kiểm kê thất bại');
            return;
        }

        toast.success(res.em || 'Hoàn thành kiểm kê thành công');
        handleClose();
        await fetchStockTake(1);
    };

    return (
        <Modal show={show} onHide={handleClose} centered className='modal-complete-stock-take'>
            <Modal.Header closeButton>
                <Modal.Title>Hoàn thành phiếu kiểm kê #{dataComplete.stockTakeID}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                {loading && <div className="text-center text-muted">Đang tải...</div>}
                {!loading && (
                    <>
                        <p>Bạn có chắc chắn muốn hoàn thành phiếu kiểm kê #{dataComplete.stockTakeID} không?</p>
                        <p className="text-muted mb-1">
                            Hệ thống sẽ tự động tạo:
                        </p>
                        <ul>
                            <li>Phiếu điều chỉnh tồn kho ({adjustCount} dòng)</li>
                            <li>Phiếu hủy ({destroyCount} dòng)</li>
                        </ul>
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

export default ModalCompleteStockTake;
